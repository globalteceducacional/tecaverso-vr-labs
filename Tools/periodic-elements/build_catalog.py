import json,pathlib,re
R=pathlib.Path(__file__).resolve().parents[2];B=R/'Assets/_Project/Laboratories/Chemistry/PeriodicBonds/Data';C=R/'Temp/periodic-import'
rows=[line.split('|') for line in (R/'Tools/periodic-elements/content-pt.tsv').read_text(encoding='utf-8-sig').splitlines() if line.strip()]
assert len(rows)==118 and [int(r[0]) for r in rows]==list(range(1,119))
photos_path=C/'photos.json' if (C/'photos.json').exists() else B/'PhotoProvenance.json'
photos={p['number']:p for p in json.loads(photos_path.read_text(encoding='utf8'))}
for photo in photos.values():
 if photo['license']=='Public domain' and not photo.get('licenseUrl'):
  photo['licenseUrl']='https://creativecommons.org/publicdomain/mark/1.0/'
catalog_code=(B.parent/'Model/PeriodicElementCatalog.cs').read_text(encoding='utf-8-sig')
names={i+1:v.split('|')[1] for i,v in enumerate(''.join(re.findall(r'"([^"]*)"',catalog_code.split('const string Data =')[1].split('public static readonly')[0])).split(';'))}
symbols={i+1:v.split('|')[0] for i,v in enumerate(''.join(re.findall(r'"([^"]*)"',catalog_code.split('const string Data =')[1].split('public static readonly')[0])).split(';'))}
captions={1:'Hidrogênio em tubo de descarga. O brilho é causado pela excitação elétrica; não é a aparência do gás em repouso.',2:'Hélio em tubo de descarga elétrica.',6:'Amostra de carbono. A aparência depende da estrutura do material.',7:'Nitrogênio em tubo de descarga elétrica.',8:'Oxigênio líquido em recipiente, a temperatura criogênica.',9:'Flúor líquido a temperatura criogênica; não representa o estado à temperatura ambiente.',10:'Neônio em tubo de descarga elétrica.',15:'Fósforo violeta: uma forma alotrópica do elemento.',16:'Enxofre nativo: amostra mineral.',17:'Cloro em recipiente de vidro.',18:'Argônio em tubo de descarga elétrica.',23:'Vanádio com camada superficial de óxido.',26:'Fragmentos de meteorito de ferro. Material natural contendo outros elementos; não é ferro puro.',29:'Cobre nativo: amostra mineral.',31:'Gálio em cristalização. Este metal funde próximo à temperatura ambiente.',35:'Bromo líquido em recipiente selado.',36:'Criptônio em tubo de descarga elétrica.',47:'Prata nativa: uma pepita natural.',54:'Xenônio em tubo de descarga elétrica.',63:'Európio com superfície oxidada.',79:'Folha de ouro: material preparado, não uma pepita.',80:'Mercúrio líquido: amostra do elemento.',83:'Cristal de bismuto com coloração superficial.',88:'Rádio depositado sobre folha de cobre. O suporte não é uma peça maciça de rádio.',90:'Tório metálico em ampola, com alteração superficial.',92:'Urânio empobrecido: material processado. Não é uraninita nem outro mineral bruto.'}
extra={61:'https://periodic-table.rsc.org/element/61/promethium',66:'https://periodic-table.rsc.org/element/66/dysprosium',67:'https://periodic-table.rsc.org/element/67/holmium',69:'https://periodic-table.rsc.org/element/69/thulium',89:'https://isotopes.gov/information/actinium-225',96:'https://planetarydata.jpl.nasa.gov/img/data/mpf/rover/mprv_0001/document/apxsinst.htm'}
captions.update({43:'Amostra de tecnécio produzida em instalação especializada.',45:'Pequena amostra de ródio metálico.',93:'Esfera de neptúnio-237 produzida em laboratório.',94:'Amostra de plutônio produzida em instalação especializada.',95:'Pequeno disco de amerício-241 visto ao microscópio.',97:'Berkélio-249 em solução. O líquido e o recipiente não são metal puro.',98:'Disco de califórnio, com dimensão da ordem de milímetros.',99:'Microamostra de einstênio-253; a fotografia tem ampliação.'})
entries=[];credits=['# Fotografias de amostras — créditos locais','', 'Importação e revisão: 09/11/2026. Metadados do catálogo sob CC BY-SA 3.0; cada fotografia conserva sua própria licença. Redimensionamento para exibição em VR. Nenhuma imagem gerada por IA é usada como amostra científica.','', 'Índice inicial de candidatos: Bowserinator/Periodic-Table-JSON (CC BY-SA 3.0). Fontes e licenças verificadas nas páginas indicadas por entrada.','']
for n,occ,app in rows:
 n=int(n);p=photos.get(n)
 status='Não foi incluída uma fotografia real com licença verificada neste acervo. A ausência da imagem não significa que o elemento não exista.'
 if n>=100:status='Sem fotografia de amostra macroscópica: este elemento é produzido em quantidades muito pequenas para formar uma peça observável. Não mostramos ilustrações como se fossem fotografias reais.'
 if n in (61,84,85,86,87):status='Sem fotografia real validada neste acervo. Foram descartadas montagens e ilustrações que poderiam ser confundidas com uma amostra. Todos os isótopos deste elemento são radioativos.'
 entry={'number':n,'occurrence':occ,'applications':app,'caption':captions.get(n,'Amostra preparada de '+names[n]+'. Não representa necessariamente sua forma de ocorrência na natureza.'),'resource':f'PeriodicSamples/{n:03}' if p else '', 'photoStatus':status,'author':p['author'] if p else '', 'license':p['license'] if p else '', 'licenseUrl':p['licenseUrl'] if p else '', 'photoSource':p['source'] if p else '', 'sources':f'PubChem / NCBI: https://pubchem.ncbi.nlm.nih.gov/element/{n}'+ ('\n'+extra[n] if n in extra else '')}
 entries.append(entry)
 if p:credits+= [f'## {n} — {symbols[n]}',entry['caption'],f'Autor: {p["author"]}',f'Licença: {p["license"]} — {p["licenseUrl"]}',f'Fonte: {p["source"]}',f'Arquivo local: Resources/PeriodicSamples/{n:03}.jpg','']
(B/'ElementReference.json').write_text(json.dumps({'revision':'2026-11-09','elements':entries},ensure_ascii=False,indent=2),encoding='utf8')
(B/'PHOTO_CREDITS.md').write_text('\n'.join(credits),encoding='utf8')
(B/'PhotoProvenance.json').write_text(json.dumps(list(photos.values()),ensure_ascii=False,indent=2),encoding='utf8')
# Store only provenance, not uncurated upstream prose, in the application repository.
evidence_path=C/'evidence.json' if (C/'evidence.json').exists() else B/'ScientificSources.json'
evidence=json.loads(evidence_path.read_text(encoding='utf8'))
(B/'ScientificSources.json').write_text(json.dumps([{'number':e['number'],'source':e['source'],'references':e['references']} for e in evidence],ensure_ascii=False,indent=2),encoding='utf8')
print(f'{len(entries)} entries; {len(photos)} photographs; {118-len(photos)} explicit photo absence explanations.')


