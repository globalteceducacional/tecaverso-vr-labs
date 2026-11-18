# Corpo humano e sistemas

Segunda sala individual offline de Biologia, conforme o [GDD aprovado](../../../../../docs/gdd-biologia-anatomia.md). Cena: `Scenes/Anatomy.unity`. Entrada: **Explorar sozinho → Biologia → Corpo humano e sistemas**. A disciplina mantém exatamente seis conteúdos; esta sala consolida os antigos laboratórios de sistemas.

## Uso

O corpo mede aproximadamente 1,7 m e permanece ancorado numa base. No console inclinado à esquerda, escolha um dos nove sistemas: Respiratório, Circulatório, Digestório, Nervoso, Esquelético, Muscular, Urinário, Linfático e Reprodutivo. Apenas um fica ativo por vez. **Pele (contexto)** mostra uma camada semitransparente independente, sem collider, inicialmente ligada.

Selecione uma estrutura por raio ou proximidade para abrir a ficha lateral com nome, função e contexto. Uma linha liga o nome à estrutura. **Fechar consulta** encerra a leitura. Selecionar outro alvo substitui a ficha.

A seleção breve permite consultar as 60 estruturas. Em **24 unidades destacáveis**, mantenha a seleção e mova a mão ou o ponto de pega do raio para aproximar a peça, virar e observar seu verso. Solte para devolvê-la à pose anatômica. O coração, os dois pulmões e os dois rins são exemplos individuais. Crânio, pelve e caixa torácica preservam os conjuntos do acervo. As 36 unidades fixas continuam consultáveis. A tabela por sistema está no [GDD](../../../../../docs/gdd-biologia-anatomia.md).

O **dial Girar corpo** fica à direita da grade, com a manopla sobre o tampo, voltada para quem o opera. O conjunto gira ancorado. Foco e seleção usam a silhueta de `Shared/Interaction`. A ficha e a devolução respeitam `MotionPreferences.Reduced`.

**Observar percurso** inicia uma das quatro sequências. Durante cada uma, o console permite pausar e retomar na mesma fase.

| Sistema | Representação | O que observar |
|---|---|---|
| Respiratório | Luz ciana avança nas vias e nos dois pulmões, em seguida inverte o sentido | Inspiração e expiração |
| Circulatório | Coração direito, pulmões, coração esquerdo, artérias até a mão e retorno venoso | Azul e vermelho indicam oxigenação. O sangue humano permanece vermelho |
| Digestório | Onda pela boca, faringe, esôfago, estômago e intestinos até a região de eliminação | Sentido do conteúdo e papel dos órgãos acessórios |
| Nervoso | Frente luminosa acompanha o membro superior direito, a medula cervical e o encéfalo | Propagação sensitiva ilustrativa |

A luz representa regiões e etapas, sem escala temporal ou simulação fisiológica quantitativa. Afastar uma peça suspende a visualização enquanto o órgão está na mão. Devolvê-la recupera o estado de reprodução ou pausa. Trocar sistema devolve a peça, fecha a consulta e encerra o percurso anterior. Nos cinco sistemas estáticos, o comando de percurso se recolhe. **Voltar ao Hub** fica sempre no console.

## Conteúdo e componentes

São 60 alvos de consulta: 9 respiratórios, 5 circulatórios, 11 digestórios, 3 nervosos, 14 esqueléticos/contextuais, 4 musculares, 6 urinários, 4 linfáticos e 4 reprodutivos. Redes e conjuntos não prometem seleção de cada vaso, nervo, músculo ou dos 206 ossos. O acervo masculino não inclui alternância para o modelo feminino.

- `AnatomyDefinitions`: mapa explícito dos grupos e textos locais, com cobertura das malhas conferida durante a autoria.
- `AnatomyLab`: sistemas exclusivos, consulta, visibilidade, dial e navegação.
- `AnatomyOrgan`: consulta XR e inspeção das unidades anatômicas elegíveis, com devolução à origem.
- `AnatomyPath`: destaques de superfície e etapas pausáveis dos quatro processos.
- `Shared/Interaction` e controles Unity: feedback comum e dial reutilizável.

O ambiente, rig, EventSystem e renderização atuais foram reutilizados sem importar a arquitetura antiga. Arte e rastreio em [Art/Biology/Anatomy](../../../Art/Biology/Anatomy/README.md). Textos consultaram as referências OpenStax listadas no GDD; não há API em runtime. A sala não oferece sincronização LAN.

## Validação

A demonstração do Unity Recorder mostra os nove sistemas, os quatro percursos com pausa, o dial e a pega, rotação e devolução do coração por um interator XR. Câmera, legendas e ponteiro são exclusivos do vídeo. A gravação fica em `Recordings/Biology`, ignorada pelo Git. [Roteiro e verificação do vídeo](../../../../../docs/recording-anatomy.md)

Verificações focadas e evidências são registradas em [docs/biology/validation.md](../../../../../docs/biology/validation.md). Capturas de revisão ficam em `GeneratedMedia/Anatomy`, ignoradas pelo Git. Não foram autorizados builds. Leitura angular, alcance sentado/em pé e desempenho em headset requerem validação com hardware; a verificação no Editor não os substitui.
