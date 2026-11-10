const fs=require('node:fs');
const path=require('node:path');
const root=__dirname;
fs.mkdirSync(path.join(root,'out'),{recursive:true});
for (const [suffix, input] of [['PT','math-pt-narration-timing.json'],['EN','math-en-narration-timing.json']]) {
 const scenes=JSON.parse(fs.readFileSync(path.join(root,'qa',input),'utf8'));
 const stamp=(s)=>{const ms=Math.round(s*1000);const h=Math.floor(ms/3600000);const m=Math.floor(ms%3600000/60000);const sec=Math.floor(ms%60000/1000);return `${String(h).padStart(2,'0')}:${String(m).padStart(2,'0')}:${String(sec).padStart(2,'0')},${String(ms%1000).padStart(3,'0')}`;};
 const srt=scenes.map((s,i)=>`${i+1}\n${stamp(s.start)} --> ${stamp(s.start+s.duration)}\n${s.text}\n`).join('\n');
 fs.writeFileSync(path.join(root,'out',`Tecaverso-Matematica-${suffix}.srt`),srt,'utf8');
}
