# DNA, RNA e informação

[Abrir a cena](Scenes/Dna.unity) · [GDD, inventário e prévias](../../../../../docs/gdd-biologia-dna.md) · [Protótipo interativo](../../../../../docs/prototypes/biologia/index.html?lab=dna)

O aluno observa a dupla hélice à esquerda, monta duas fitas independentes na bancada inclinada e consulta o modelo do cromossomo à direita. A atividade individual funciona offline.

## Uso

1. Leia os sentidos do molde e da fita a completar nas extremidades da bancada. Pegue uma pastilha pela mão ou pelo raio XR. Os quatro encaixes podem ser preenchidos em qualquer ordem.
2. Aproxime a base de um encaixe. A linha de apoio identifica um par compatível ou indica qual letra o molde requer. A peça permanece na mão até que o aluno a solte. Ao soltá-la fora de um encaixe compatível, ela volta ao berço.
3. Alterne DNA e RNA. As quatro posições de cada montagem ficam guardadas enquanto a sala está aberta. Pegue novamente uma peça encaixada para retirá-la. T e U ocupam o mesmo berço em suas respectivas atividades.
4. Gire a manopla para observar a hélice. Selecione a hélice ou o cromossomo para abrir a consulta lateral. Restaurar pede confirmação quando houver bases encaixadas em qualquer uma das fitas.

Ao ultrapassar 1,6 m do centro da bancada com uma peça, ela se solta e retorna ao berço identificado.

## Modelos e dependências

A bancada usa um molde escolar fixo de quatro bases. As letras na superfície complementam as cores, e as ampliações da hélice e do cromossomo são independentes. O cromossomo representa a forma duplicada e condensada.

Os dois arquivos FBX em `Assets/_Project/Art/Biology/Dna/Models` são cópias dos originais preservados em `SourceAssets/BiologyLegacy/Assets/_Project/Models/Biologia`. A hélice utiliza a variante `DNA_01_grp`. A cena referencia as malhas locais e os materiais URP da apresentação.

| Fonte preservada | GUID do modelo na sala |
|---|---|
| `DNA/dna_strand_set.FBX` | `708d535106c0451d83a83dc6a7e2648e` |
| `Microbios/Chromosome/Chromosome FBX.FBX` | `410f26ee43994cbfbc3655391764633e` |

## Verificação

`Tecaverso.Editor.DnaValidation.Run()` exercita o fluxo de encaixe por eventos XR em Play Mode na cena DNA. Verifica a recusa de A–A, as duas montagens, a retirada e reposição, a consulta e a restauração. O teste começa com a bancada vazia e termina na mesma condição. O desempenho, o alcance e a leitura em headset físico exigem homologação separada.
