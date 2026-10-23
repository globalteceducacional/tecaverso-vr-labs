using System;
using System.Collections.Generic;

namespace Tecaverso.Labs.ResistorCircuits
{
    // The electrical model has no dependency on Rigidbody positions or cable animation.
    public static class CircuitGraph
    {
        public readonly struct Branch
        {
            public readonly int A, B;
            public readonly double Ohms;
            public Branch(int a,int b,double ohms){A=a;B=b;Ohms=ohms;}
        }
        public sealed class Result
        {
            public readonly double[] Voltage;
            public readonly double[] Current;
            public readonly bool Powered;
            public readonly bool ShortCircuit;
            public Result(double[] voltage,double[] current,bool powered,bool shortCircuit)
            {Voltage=voltage;Current=current;Powered=powered;ShortCircuit=shortCircuit;}
        }

        public static Result Solve(int terminalCount,IReadOnlyList<(int a,int b)> wires,IReadOnlyList<Branch> branches,int plus,int minus,double sourceVolts)
        {
            var parent=new int[terminalCount];for(int i=0;i<terminalCount;i++)parent[i]=i;
            int Find(int x){while(parent[x]!=x){parent[x]=parent[parent[x]];x=parent[x];}return x;}
            foreach(var (a,b) in wires)parent[Find(a)]=Find(b);
            int positive=Find(plus),negative=Find(minus);
            var volts=new double[terminalCount];var currents=new double[branches.Count];
            if(positive==negative)return new Result(volts,currents,false,true);
            var nodes=new Dictionary<int,int>();
            for(int i=0;i<terminalCount;i++){int root=Find(i);if(root!=negative&&root!=positive&&!nodes.ContainsKey(root))nodes[root]=nodes.Count;}
            int n=nodes.Count;var matrix=new double[n,n+1];
            foreach(var branch in branches)
            {
                int a=Find(branch.A),b=Find(branch.B);if(a==b||branch.Ohms<=0)continue;
                double g=1/branch.Ohms;
                if(nodes.TryGetValue(a,out int ai)){matrix[ai,ai]+=g;if(nodes.TryGetValue(b,out int bi))matrix[ai,bi]-=g;else if(b==positive)matrix[ai,n]+=g*sourceVolts;}
                if(nodes.TryGetValue(b,out int bj)){matrix[bj,bj]+=g;if(nodes.TryGetValue(a,out int aj))matrix[bj,aj]-=g;else if(a==positive)matrix[bj,n]+=g*sourceVolts;}
            }
            // Unconnected islands have no determined potential; ground them at zero for display.
            for(int i=0;i<n;i++)if(Math.Abs(matrix[i,i])<1e-12)matrix[i,i]=1;
            for(int p=0;p<n;p++)
            {
                int row=p;for(int r=p+1;r<n;r++)if(Math.Abs(matrix[r,p])>Math.Abs(matrix[row,p]))row=r;
                if(Math.Abs(matrix[row,p])<1e-12)continue;
                if(row!=p)for(int c=p;c<=n;c++)(matrix[p,c],matrix[row,c])=(matrix[row,c],matrix[p,c]);
                double pivot=matrix[p,p];for(int c=p;c<=n;c++)matrix[p,c]/=pivot;
                for(int r=0;r<n;r++)if(r!=p){double factor=matrix[r,p];for(int c=p;c<=n;c++)matrix[r,c]-=factor*matrix[p,c];}
            }
            for(int i=0;i<terminalCount;i++){int root=Find(i);volts[i]=root==positive?sourceVolts:root==negative?0:nodes.TryGetValue(root,out int index)?matrix[index,n]:0;}
            double total=0;
            for(int i=0;i<branches.Count;i++)
            {
                var b=branches[i];if(b.Ohms<=0)continue;
                currents[i]=(volts[b.A]-volts[b.B])/b.Ohms;
                if(Find(b.A)==positive&&Find(b.B)!=positive)total+=currents[i];
                if(Find(b.B)==positive&&Find(b.A)!=positive)total-=currents[i];
            }
            if(total>10)return new Result(new double[terminalCount],new double[branches.Count],false,true);
            return new Result(volts,currents,total>1e-6,false);
        }
    }
}
