# Apresentação comum dos laboratórios

`TecaversoVR.asset` é o pipeline URP padrão em Graphics Settings e nos três níveis de qualidade. Reaproveita o renderer Forward do projeto, com MSAA 4×, render scale 1 e HDR habilitado. MSAA já estava em 4× no pipeline anterior; esta configuração o consolida em um asset próprio do projeto.

`TecaversoLearningProfile.asset` é o **Default Volume Profile** nas configurações globais da URP. Aplica-se a todas as disciplinas, inclusive ao Hub preservado no carregamento LAN aditivo, sem GameObjects de Volume duplicados.

- Tonemapping Neutral; exposição, balanço de branco e saturação neutros; contraste +3.
- Bloom: intensidade 0,08, limiar 1,2, scatter 0,45, resolução Quarter, máximo de quatro iterações e filtro de alta qualidade desligado. O limiar concentra o brilho nas áreas emissivas, preservando a leitura dos textos e vetores.
- Sem motion blur, depth of field, aberração cromática, distorção, vinheta ou granulação.
- As câmeras de Hub, ObliqueLaunch e InclinedPlane têm Allow MSAA, Allow HDR e Render Post Processing habilitados. Antialiasing no componente Universal Additional Camera Data fica em None: a suavização é feita pelo MSAA do pipeline.

Para uma nova disciplina, use o pipeline e o perfil globais e habilite essas três opções na câmera do rig. Não é necessário criar outro Volume. Não substitua o perfil global por um perfil temático que altere as cores científicas.

Configuração visual validada no Editor. HDR, MSAA e bloom ainda precisam de medição no headset mínimo, especialmente em Android; este ajuste não certifica frequência de quadros ou conforto. Não foi gerado build.
