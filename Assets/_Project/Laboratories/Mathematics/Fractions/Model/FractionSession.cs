using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
namespace Tecaverso.Labs.Fractions
{
    // Every supported denominator divides 120. Integers make equality and capacity exact.
    public sealed class FractionSession
    {
        public static readonly int[] Denominators={1,2,3,4,5,6,8,10,12};
        public const int Unit=120,Capacity=240;
        readonly List<int>[] rows={new(),new()};
        readonly List<int[][]> history=new();
        public readonly int[] Recipe=new int[2];
        public int Multiplier {get;set;}=1;
        public IReadOnlyList<int> Row(int row)=>rows[row];
        public int Total(int row)=>rows[row].Sum(d=>Unit/d);
        public bool CanUndo=>history.Count>0;
        public bool HasWork=>rows.Any(r=>r.Count>0)||Recipe.Any(n=>n>0);
        public bool Fits(int row,int denominator)=>Denominators.Contains(denominator)&&rows[row].Count<24&&Total(row)+Unit/denominator<=Capacity;
        void Save(){history.Add(rows.Select(r=>r.ToArray()).ToArray());if(history.Count>10)history.RemoveAt(0);}
        public bool Add(int row,int denominator){if(!Fits(row,denominator))return false;Save();rows[row].Add(denominator);return true;}
        public bool Remove(int row,int index){if(index<0||index>=rows[row].Count)return false;Save();rows[row].RemoveAt(index);return true;}
        public bool Undo(){if(!CanUndo)return false;var previous=history[history.Count-1];history.RemoveAt(history.Count-1);for(int i=0;i<2;i++){rows[i].Clear();rows[i].AddRange(previous[i]);}return true;}
        public void ClearRows(){if(rows.All(r=>r.Count==0))return;Save();foreach(var r in rows)r.Clear();}
        public bool AddToken(int category){if(Recipe[category]>=10)return false;Recipe[category]++;return true;}
        public bool RemoveToken(int category){if(Recipe[category]==0)return false;Recipe[category]--;return true;}
        public void ClearRecipe(){Array.Clear(Recipe,0,2);}
        public bool SameRatio=>Recipe[0]>0&&Recipe[1]>0&&Recipe[0]*3==Recipe[1]*2;
        public bool TargetReached=>Recipe[0]==2*Multiplier&&Recipe[1]==3*Multiplier;
        public static int Gcd(int a,int b){while(b!=0){int c=a%b;a=b;b=c;}return a;}
        public static string Fraction(int numerator,int denominator){if(numerator==0)return "0";int g=Gcd(numerator,denominator);numerator/=g;denominator/=g;return denominator==1?numerator.ToString():numerator+"/"+denominator;}
        public static string Rounded(int numerator,int denominator,int digits,int scale=1){int factor=(int)Math.Pow(10,digits);bool exact=(numerator*scale*factor)%denominator==0;return (exact?"= ":"≈ ")+((decimal)numerator*scale/denominator).ToString("0."+new string('#',digits),CultureInfo.GetCultureInfo("pt-BR"));}
        public string TotalText(int row,bool decimals){int n=Total(row);string text=Fraction(n,Unit);if(n>Unit&&n%Unit!=0)text+=" = "+n/Unit+" + "+Fraction(n%Unit,Unit);return text+(decimals?"\n"+Rounded(n,Unit,3)+" · "+Rounded(n,Unit,1,100)+"%":"");}
        public string RecipeText(){int a=Recipe[0],b=Recipe[1],total=a+b;string text="A : B = "+a+" : "+b+" · Total: "+total;if(total==0)return text+"\nParticipações: —\nReceita vazia";int g=Gcd(a,b);text+="\nRazão reduzida: "+a/g+" : "+b/g+"\nA: "+Fraction(a,total)+" "+Rounded(a,total,1,100)+"% · B: "+Fraction(b,total)+" "+Rounded(b,total,1,100)+"%";return text+"\n"+(a==0||b==0?"Receita incompleta":SameRatio?"Mesma proporção (2:3)":"Proporção diferente de 2:3")+"\n"+(TargetReached?"Quantidade da referência atingida":"Referência: "+2*Multiplier+" A e "+3*Multiplier+" B");}
    }
}
