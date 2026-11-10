using System;

namespace Tecaverso.Labs.Chemistry.PeriodicBonds
{
    public readonly struct PeriodicElement
    {
        public readonly int atomicNumber, period, group;
        public readonly string symbol, name;
        public bool IsLanthanide => atomicNumber >= 57 && atomicNumber <= 71;
        public bool IsActinide => atomicNumber >= 89 && atomicNumber <= 103;
        public PeriodicElement(int atomicNumber, string symbol, string name, int period, int group)
        { this.atomicNumber=atomicNumber;this.symbol=symbol;this.name=name;this.period=period;this.group=group; }
    }

    // Symbols and Brazilian Portuguese names checked against CRQ-SP's 118-element table:
    // https://crqsp.org.br/tabelaperiodica/
    public static class PeriodicElementCatalog
    {
        const string Data =
            "H|Hidrogênio;He|Hélio;Li|Lítio;Be|Berílio;B|Boro;C|Carbono;N|Nitrogênio;O|Oxigênio;F|Flúor;Ne|Neônio;"+
            "Na|Sódio;Mg|Magnésio;Al|Alumínio;Si|Silício;P|Fósforo;S|Enxofre;Cl|Cloro;Ar|Argônio;"+
            "K|Potássio;Ca|Cálcio;Sc|Escândio;Ti|Titânio;V|Vanádio;Cr|Crômio;Mn|Manganês;Fe|Ferro;Co|Cobalto;Ni|Níquel;Cu|Cobre;Zn|Zinco;Ga|Gálio;Ge|Germânio;As|Arsênio;Se|Selênio;Br|Bromo;Kr|Criptônio;"+
            "Rb|Rubídio;Sr|Estrôncio;Y|Ítrio;Zr|Zircônio;Nb|Nióbio;Mo|Molibdênio;Tc|Tecnécio;Ru|Rutênio;Rh|Ródio;Pd|Paládio;Ag|Prata;Cd|Cádmio;In|Índio;Sn|Estanho;Sb|Antimônio;Te|Telúrio;I|Iodo;Xe|Xenônio;"+
            "Cs|Césio;Ba|Bário;La|Lantânio;Ce|Cério;Pr|Praseodímio;Nd|Neodímio;Pm|Promécio;Sm|Samário;Eu|Európio;Gd|Gadolínio;Tb|Térbio;Dy|Disprósio;Ho|Hólmio;Er|Érbio;Tm|Túlio;Yb|Itérbio;Lu|Lutécio;"+
            "Hf|Háfnio;Ta|Tântalo;W|Tungstênio;Re|Rênio;Os|Ósmio;Ir|Irídio;Pt|Platina;Au|Ouro;Hg|Mercúrio;Tl|Tálio;Pb|Chumbo;Bi|Bismuto;Po|Polônio;At|Astato;Rn|Radônio;"+
            "Fr|Frâncio;Ra|Rádio;Ac|Actínio;Th|Tório;Pa|Protactínio;U|Urânio;Np|Netúnio;Pu|Plutônio;Am|Amerício;Cm|Cúrio;Bk|Berquélio;Cf|Califórnio;Es|Einstênio;Fm|Férmio;Md|Mendelévio;No|Nobélio;Lr|Laurêncio;"+
            "Rf|Rutherfórdio;Db|Dúbnio;Sg|Seabórgio;Bh|Bóhrio;Hs|Hássio;Mt|Meitnério;Ds|Darmstádtio;Rg|Roentgênio;Cn|Copernício;Nh|Nihônio;Fl|Fleróvio;Mc|Moscóvio;Lv|Livermório;Ts|Tenesso;Og|Oganessônio";

        public static readonly PeriodicElement[] All = Create();
        static PeriodicElement[] Create()
        {
            var rows=Data.Split(';');
            if(rows.Length!=118)throw new InvalidOperationException("O catálogo deve conter 118 elementos.");
            var result=new PeriodicElement[118];
            for(int i=0;i<rows.Length;i++)
            {
                var fields=rows[i].Split('|');
                if(fields.Length!=2)throw new InvalidOperationException("Elemento inválido: " + (i+1));
                int number=i+1,period=Period(number),group=Group(number);
                result[i]=new PeriodicElement(number,fields[0],fields[1],period,group);
            }
            return result;
        }
        public static int Period(int number)
        {
            if(number<1||number>118)throw new ArgumentOutOfRangeException(nameof(number));
            if(number<=2)return 1;if(number<=10)return 2;if(number<=18)return 3;
            if(number<=36)return 4;if(number<=54)return 5;if(number<=86)return 6;return 7;
        }
        public static int Group(int number)
        {
            int period=Period(number);
            if(period==1)return number==1?1:18;
            if(period==2||period==3)
            {int start=period==2?3:11;int offset=number-start;return offset<2?offset+1:offset+11;}
            if(period==4||period==5)return number-(period==4?19:37)+1;
            int startLarge=period==6?55:87;
            if(number<startLarge+2)return number-startLarge+1;
            if(number<startLarge+17)return 0; // f-block, shown on the adjacent wall
            return number-(startLarge+17)+4;
        }
    }
}
