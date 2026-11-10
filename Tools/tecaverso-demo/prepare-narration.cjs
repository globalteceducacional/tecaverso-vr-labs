const fs=require('node:fs');
const cp=require('node:child_process');
const path=require('node:path');
const bin=path.join(__dirname,'node_modules/@remotion/compositor-win32-x64-msvc');
const english=process.argv.includes('--english');
const math=process.argv.includes('--math');
const francisca=process.argv.includes('--francisca');
const prefix=math?(english?'math-en-':'math-pt-'):english?'english-':francisca?'francisca-':'';
const scenes=JSON.parse(fs.readFileSync(math?(english?'math-narration-en.json':'math-narration.json'):(english?'narration-en.json':'narration.json'),'utf8').replace(/^\uFEFF/,''));
function run(tool,args){const p=cp.spawnSync(path.join(bin,tool+'.exe'),args,{encoding:'utf8'});if(p.status!==0)throw Error(p.stderr);return p.stdout;}
function readWav(file){const b=fs.readFileSync(file);let data,channels,rate,bits;for(let o=12;o+8<=b.length;){const id=b.toString('ascii',o,o+4),size=b.readUInt32LE(o+4);if(id==='fmt '){channels=b.readUInt16LE(o+10);rate=b.readUInt32LE(o+12);bits=b.readUInt16LE(o+22);}if(id==='data'){data=b.subarray(o+8,o+8+size);break;}o+=8+size+(size%2);}if(channels!==2||rate!==48000||bits!==16||!data)throw Error('Expected stereo PCM16 48kHz: '+file);return data;}
function wav(data){const b=Buffer.alloc(44+data.length);b.write('RIFF');b.writeUInt32LE(b.length-8,4);b.write('WAVEfmt ',8);b.writeUInt32LE(16,16);b.writeUInt16LE(1,20);b.writeUInt16LE(2,22);b.writeUInt32LE(48000,24);b.writeUInt32LE(192000,28);b.writeUInt16LE(4,32);b.writeUInt16LE(16,34);b.write('data',36);b.writeUInt32LE(data.length,40);data.copy(b,44);return b;}
const total=Math.round(90.5*48000), narration=Buffer.alloc(total*4);
for(const scene of scenes){
 const input=math?'public/voiceover/'+(english?'math-english-source/':'math-francisca-source/')+scene.id+'.mp3':english?'public/voiceover/english-source/'+scene.id+'.mp3':francisca?'public/voiceover/francisca-source/'+scene.id+'.mp3':'qa/'+scene.id+'-raw.wav',output='public/voiceover/'+prefix+scene.id+'.wav';
 const duration=Number(run('ffprobe',['-v','error','-show_entries','format=duration','-of','csv=p=0',input]));
 const speed=Math.max(1,duration/(scene.end-scene.start-.06));
 if(speed>1.12)throw Error('Rewrite rather than rush: '+scene.id);
 run('ffmpeg',['-v','error','-y','-i',input,'-af',`atempo=${speed},loudnorm=I=-18:TP=-3:LRA=7`,'-ar','48000','-ac','2','-c:a','pcm_s16le',output]);
 const data=readWav(output);scene.duration=data.length/192000;
 if(scene.start+scene.duration>scene.end+.02)throw Error('Speech exceeds scene: '+scene.id);
 data.copy(narration,Math.round(scene.start*48000)*4);
 console.log(scene.id+': '+scene.duration.toFixed(2)+'s, from '+scene.start+'s');
}
const ambient=readWav('public/audio/'+(math?'tecaverso-ambient.wav':'tecaverso-ambient.wav'));const ducked=Buffer.alloc(ambient.length);
const smooth=x=>{x=Math.max(0,Math.min(1,x));return x*x*(3-2*x)};
for(let i=0;i<total;i++){
 const t=i/48000;let speech=0;
 for(const s of scenes){speech=Math.max(speech,smooth((t-(s.start-.35))/.35)*(1-smooth((t-(s.start+s.duration))/.65)));}
 const gain=.65-(.65-.28)*speech;
 for(let ch=0;ch<2;ch++){const offset=i*4+ch*2;ducked.writeInt16LE(Math.round(ambient.readInt16LE(offset)*gain),offset);}
}
fs.writeFileSync('public/audio/'+prefix+'tecaverso-ambient-ducked.wav',wav(ducked));
fs.writeFileSync('public/voiceover/'+prefix+'tecaverso-narracao.wav',wav(narration));
fs.writeFileSync('qa/'+prefix+'narration-timing.json',JSON.stringify(scenes,null,2));
