using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ILOG.Concert;
using ILOG.CPLEX;

namespace learnCplex
{
    class Program
    {
        static void Main(string[] args)
        {
            #region 将要求解问题参数提取出来
            double[] c = { 5, 12 };
            double[] lb = { 0, 0 };
            double[] ub = { System.Double.MaxValue, System.Double.MaxValue };
            double[][] A_le = {new double[] {1,-1},new double[] {2,1}};
            double[] b_le = { 1, 4 };
            double[][] A_eq =  { };
            double[] b_eq = { };
            NumVarType[] xt = new NumVarType[2] { NumVarType.Float,NumVarType.Float};
            #endregion

            Cplex cplex = new Cplex();//建立问题模型
            INumVar[][] var = new INumVar[1][];//用于求解后调用或查看结果
            IRange[][] rng = new IRange[1][];//用于求解后调用或查看松弛程度

            int n = lb.Length; //变量个数，亦可赋值为c.Length或ub.Length
            INumVar[] x = cplex.NumVarArray(n, lb, ub, xt);//添加决策变量
                                                           //若退化为纯线性规划则可省略xt，改写为 
                                                           //INumVar[] x = cplex.NumVarArray(n, lb, ub); 

            var[0] = x;//加引用(指针变化)，两个可以等同

            cplex.AddMaximize(cplex.ScalProd(x, c));//添加优化目标
            //若需要最小化目标则改为AddMinimize

            //下面将定义约束
            rng[0] = new IRange[A_le.Length + A_eq.Length];
            //尝试添加约束的一种方法
            for (int j = 0; j < A_le.Length; j++)//不等式约束
            {
                rng[0][j] = cplex.AddLe(cplex.ScalProd(x, A_le[j]), b_le[j]);
            }
            for (int j = 0; j < A_eq.Length; j++)//等式约束
            {
                rng[0][j + A_le.Length] = cplex.AddEq(cplex.ScalProd(x, A_eq[j]), b_eq[j]);
            }

            if (cplex.Solve())//以下列出求解成功后常用的成员
            {
                double[] result = cplex.GetValues(var[0]);//得到决策变量值
                Console.WriteLine("Solution status = " + cplex.GetStatus());
                //求解状态
                Console.WriteLine("Solution value = " + cplex.ObjValue); //目标最优值
                for (int j = 0; j < result.Length; j++)
                {
                    Console.WriteLine("result[" + j + "] = " + result[j]);
                }
            }
            cplex.End();
            Console.ReadKey();
        }
    }
}
