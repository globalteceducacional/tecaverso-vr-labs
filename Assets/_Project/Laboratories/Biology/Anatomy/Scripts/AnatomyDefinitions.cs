using System;
using System.Collections.Generic;
using System.Linq;

namespace Tecaverso.Labs.Biology.Anatomy
{
    public static class AnatomyDefinitions
    {
        public static readonly string[] Ids={"resp","circ","dig","nerv","skel","muscle","renal","lymph","repro"};
        public static readonly string[] Titles={"Respiratório","Circulatório","Digestório","Nervoso","Esquelético","Muscular","Urinário","Linfático","Reprodutivo"};
        public sealed class Part
        {
            public string title,function,context;
            public string[] meshes;
        }
        public static List<Part> Parts(string system,IEnumerable<string> source)
        {
            var names=source.ToArray();var parts=new List<Part>();
            void Add(string title,string function,string context,params string[] meshes)
            {
                if(meshes.Length==0 || meshes.Any(m=>!names.Contains(m)))throw new InvalidOperationException("Mapa anatômico incompleto: "+system+" / "+title);
                parts.Add(new Part{title=title,function=function,context=context,meshes=meshes});
            }
            string[] Prefix(string prefix)=>names.Where(n=>n.StartsWith(prefix,StringComparison.Ordinal)).ToArray();
            switch(system)
            {
                case "resp":
                    Add("Cavidade nasal","Filtra, aquece e umidifica o ar inspirado.","Uma das entradas do ar no sistema respiratório.","NasalCavity");
                    Add("Cavidade oral","Também permite passagem de ar.","É compartilhada com o sistema digestório.","OralCavity");
                    Add("Faringe","Conduz ar em direção à laringe.","Também participa da passagem do alimento; vias distintas seguem a partir dessa região.","Pharynx");
                    Add("Laringe","Conecta a faringe à traqueia e participa da produção da voz.","As duas malhas representam aspectos internos e externos da mesma região.","Larynx_Internal","Larynx_External");
                    Add("Traqueia","Conduz ar entre a laringe e os brônquios.","Não é o tubo percorrido pelo alimento.","Trachea");
                    Add("Brônquios","Distribuem ar para os pulmões.","A ramificação fina e os alvéolos não são individualmente selecionáveis.","Bronchi");
                    Add("Pulmão esquerdo","Participa das trocas gasosas entre ar e sangue.","Esquerdo é o lado da pessoa representada, não o de quem observa.","Lung_Left");
                    Add("Pulmão direito","Participa das trocas gasosas entre ar e sangue.","O percurso do ar é esquemático, sem simular ventilação quantitativa.","Lung_Right");
                    Add("Diafragma","Sua contração auxilia a inspiração ao alterar o volume torácico.","Aqui é mostrado estaticamente: a malha não contrai durante o percurso.","Diaphragm");break;
                case "circ":
                    Add("Coração","Impulsiona sangue pelos circuitos pulmonar e sistêmico.","A malha não separa câmaras ou válvulas para consulta; não simula batimentos ou pressão.","Heart");
                    Add("Rede arterial","Transporta sangue para fora do coração.","A cor vermelha é convencional. Artérias pulmonares transportam sangue com menos oxigênio.",Prefix("ArterialSystem_"));
                    Add("Rede venosa","Conduz sangue de volta ao coração.","A cor azul é convencional; o sangue humano não é azul. Veias pulmonares trazem sangue oxigenado.",Prefix("VenousSystem_"));
                    Add("Vasos associados ao coração","Conectam o coração à circulação e irrigam seu tecido.","Estes grupos não oferecem identificação individual de cada vaso.","Heart_Vessels","Heart_Vessels_Big");
                    Add("Vasos da região pulmonar","Participam da circulação junto às estruturas respiratórias.","É uma rede vascular; não corresponde ao trajeto do ar.","Bronchi_Vessels");break;
                case "dig":
                    Add("Cavidade oral","Recebe o alimento e participa da mastigação e mistura com saliva.","É o início do trajeto alimentar representado.","OralCavity");
                    Add("Glândulas salivares","Produzem saliva, que umedece o alimento e auxilia a digestão.","São órgãos acessórios: o alimento não percorre o interior dessas glândulas.","Glands_Parotid","Glands_Mandibular_Lingual");
                    Add("Faringe","Participa da condução do alimento durante a deglutição.","É uma região compartilhada pelas vias digestória e respiratória.","Pharynx");
                    Add("Esôfago","Conduz o alimento da faringe ao estômago.","As malhas da cabeça e do tórax formam um mesmo trajeto.","Esophagus_Head","Esophagus_Torso");
                    Add("Estômago","Armazena, mistura e participa da digestão do alimento.","A onda de cor mostra a etapa gástrica do percurso. O conteúdo segue para o intestino delgado.","Stomach");
                    Add("Intestino delgado","É importante para a digestão e absorção da maior parte dos nutrientes.","O percurso simplifica as numerosas alças do modelo.","Small_Inestines");
                    Add("Intestino grosso","Participa da absorção de água e da formação das fezes.","A onda percorre as regiões do cólon em sequência e termina na região de eliminação.","Colon");
                    Add("Fígado","Produz bile e participa do metabolismo de nutrientes.","Órgão acessório: o alimento não passa por dentro dele no tubo digestório.","Liver");
                    Add("Pâncreas","Produz secreções digestivas e hormônios.","Sua participação endócrina é contextual; esta sala não simula secreções.","Pancreas");
                    Add("Vesícula biliar","Armazena e concentra a bile produzida pelo fígado.","Órgão acessório ao trajeto alimentar.","Gallbladder");
                    Add("Via biliar","Conduz bile em direção ao intestino.","A bile auxilia o processamento de lipídios; o alimento não percorre esse ducto.","Bile_Duct");break;
                case "nerv":
                    Add("Encéfalo","Integra informações e participa do controle das funções do organismo.","O conjunto inclui regiões encefálicas; não isola cada núcleo ou via nervosa.",names.Where(n=>!n.StartsWith("NervousSystem_")&&!n.StartsWith("SpinalCord_")).ToArray());
                    Add("Medula espinal","Conduz sinais e participa de circuitos reflexos.","Não confundir medula espinal com coluna vertebral ou medula óssea.",Prefix("SpinalCord_"));
                    Add("Nervos periféricos","Conectam estruturas do corpo ao sistema nervoso central.","O percurso é um exemplo sensitivo em direção ao encéfalo; não é fluxo de matéria.",Prefix("NervousSystem_"));break;
                case "skel":
                    var titles=new Dictionary<string,string>{{"Skeleton_Skull","Crânio"},{"Skeleton_Spine","Coluna vertebral"},{"Skeleton_Rib_Cage","Caixa torácica"},{"Skeleton_Pelvis","Pelve"},{"Skeleton_Arm_Left","Ossos do braço esquerdo"},{"Skeleton_Arm_Right","Ossos do braço direito"},{"Skeleton_Hand_Left","Ossos da mão esquerda"},{"Skeleton_Hand_Right","Ossos da mão direita"},{"Skeleton_Leg_Left","Ossos do membro inferior esquerdo"},{"Skeleton_Leg_Right","Ossos do membro inferior direito"},{"Skeleton_Foot_Left","Ossos do pé esquerdo"},{"Skeleton_Foot_Right","Ossos do pé direito"}};
                    foreach(var name in Prefix("Skeleton_"))Add(titles[name],"Participa da sustentação, proteção ou do movimento do corpo.","A seleção representa um grupo anatômico; não individualiza todos os ossos.",name);
                    Add("Discos intervertebrais","Contribuem para mobilidade e distribuição de cargas entre vértebras.","São estruturas fibrocartilaginosas; não são ossos.","Intervertebral_Discs");
                    Add("Dentes","Participam da mastigação.","São contexto do modelo craniano; dentes não são ossos.","Teeth_Top","Teeth_Bottom");break;
                case "muscle":
                    var head=names.Where(n=>n.Contains("Neck")||n.Contains("Eye")||n.Contains("Craniofacial")).ToArray();
                    var arms=names.Where(n=>n.Contains("UpperArm")||n.Contains("Forearm")||n=="Muscles_UpperLimb").ToArray();
                    var legs=names.Where(n=>n.Contains("Hip_")||n.Contains("Calf")||n.Contains("Shin_Foot")).ToArray();
                    Add("Músculos da cabeça e pescoço","Participam de movimentos da cabeça, face e olhos.","Grupo visual; músculos individuais não são isolados.",head);
                    Add("Músculos dos membros superiores","Atuam nos movimentos dos membros superiores.","A malha não possui animação de contração ou articulação.",arms);
                    Add("Músculos dos membros inferiores","Atuam na postura e nos movimentos dos membros inferiores.","Grupo anatômico estático; não simula marcha.",legs);
                    Add("Musculatura do tronco","Participa da postura e de movimentos do tronco.","Inclui grupos profundos e superficiais do acervo.",names.Except(head).Except(arms).Except(legs).ToArray());break;
                case "renal":
                    Add("Rim esquerdo","Participa da filtração do sangue e da formação da urina.","As glândulas suprarrenais são endócrinas e não integram esta seleção urinária.","Kidney_Left");
                    Add("Rim direito","Participa da regulação de água, sais e outras substâncias.","A malha não representa néfrons individualmente.","Kidney_Right");
                    Add("Ureter esquerdo","Conduz urina do rim à bexiga.","Não confundir ureter com uretra.","Urinary_Ureter_Left");
                    Add("Ureter direito","Conduz urina do rim à bexiga.","Representação anatômica estática.","Urinary_Ureter_Right");
                    Add("Bexiga urinária","Armazena urina antes da eliminação.","Não há enchimento ou esvaziamento simulado.","Bladder");
                    Add("Uretra e próstata — conjunto","A uretra conduz urina ao exterior; a próstata é uma glândula reprodutiva.","O acervo reúne as duas estruturas numa mesma malha; elas não são o mesmo órgão.","Prostate_Uretra");break;
                case "lymph":
                    Add("Linfonodos","Participam da filtragem da linfa e da resposta imune.","O alvo reúne grupos de linfonodos do modelo.",Prefix("LymphaticNodes_"));
                    Add("Vasos linfáticos","Transportam linfa e auxiliam o retorno de líquido à circulação.","A malha apresenta uma rede simplificada, sem fluxo quantitativo.",Prefix("LymphaticSystem_"));
                    Add("Baço","Participa da resposta imune e da filtragem do sangue.","É um órgão linfoide; não faz parte do tubo digestório.","Spleen");
                    Add("Timo","Participa da maturação de linfócitos T.","Representação anatômica; não simula células de defesa.","Thymus");break;
                case "repro":
                    Add("Estruturas reprodutivas internas","Participam da produção e condução de componentes do sistema reprodutivo masculino.","Grupo do modelo; seus componentes não são individualizados.","Internal_Genetalia_Left","Internal_Genetalia_Right");
                    Add("Genitália externa masculina","Integra a anatomia externa do sistema reprodutivo masculino.","Representação educativa; o acervo feminino não está adaptado nesta entrega.","External_Genetalia");
                    Add("Próstata e uretra — conjunto","A próstata contribui para o líquido seminal; a uretra é uma via de condução.","Uma única malha reúne estruturas distintas, compartilhadas com o modelo urinário.","Prostate_Uretra");
                    Add("Músculo cremáster","Participa do posicionamento dos testículos.","É um músculo associado à região reprodutiva, mostrado estaticamente.","Cremaster");break;
                default:throw new ArgumentOutOfRangeException(nameof(system));
            }
            var mapped=parts.SelectMany(p=>p.meshes).ToArray();
            if(mapped.Length!=names.Length || mapped.Distinct().Count()!=names.Length || names.Except(mapped).Any())throw new InvalidOperationException("Peça sem mapa ou duplicada: "+system);
            return parts;
        }
    }
}
