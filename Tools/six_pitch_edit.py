from pathlib import Path
p=Path('Assets/Scripts/UI/PixelPitch.cs');s=p.read_text()
s=s.replace('new Color32(53,109,42,255):new Color32(63,124,45,255)','new Color32(40,100,47,255):new Color32(49,115,49,255)')
s=s.replace('Quad(vh,x,y,2,3,new Color32(111,151,70,45));','var near=Mathf.InverseLerp(r.yMax,r.yMin,y);\n                Quad(vh,x,y,1+near*2,1+near*2,new Color32(115,158,76,(byte)(22+near*16)));')
s=s.replace('new Color32(210,225,179,255)','new Color32(226,234,202,255)')
s=s.replace('Quad(vh,Mathf.Min(x,next)-1.5f,Mathf.Min(y,nextY)-1.5f,Mathf.Abs(next-x)+3,Mathf.Abs(nextY-y)+3,line);','Line(vh,new Vector2(x,y),new Vector2(next,nextY),3,line);')
a=s.index('            Quad(vh,-5,spotY-2');b=s.index('        private static void Quad',a)
s=s[:a]+'''            // Elliptical spot follows the pitch perspective.
            var start=vh.currentVertCount;vh.AddVert(new Vector2(0,spotY),line,Vector2.zero);
            for(var i=0;i<20;i++){var a=i*Mathf.PI*2/20;vh.AddVert(new Vector2(Mathf.Cos(a)*4.5f,spotY+Mathf.Sin(a)*2.5f),line,Vector2.zero);}
            for(var i=0;i<20;i++)vh.AddTriangle(start,start+1+i,start+1+(i+1)%20);
        }
        private static void Box(VertexHelper vh,float top,float backWidth,float frontWidth,float depth,Color32 color)
        {
            Line(vh,new Vector2(-frontWidth,top-depth),new Vector2(frontWidth,top-depth),3.5f,color);
            for(var side=-1;side<=1;side+=2)
                Line(vh,new Vector2(side*backWidth,top),new Vector2(side*frontWidth,top-depth),3.5f,color);
        }
        private static void Line(VertexHelper vh,Vector2 a,Vector2 b,float width,Color32 color)
        {
            var n=new Vector2(-(b-a).y,(b-a).x).normalized*width*.5f;var i=vh.currentVertCount;
            vh.AddVert(a-n,color,Vector2.zero);vh.AddVert(a+n,color,Vector2.zero);
            vh.AddVert(b+n,color,Vector2.zero);vh.AddVert(b-n,color,Vector2.zero);
            vh.AddTriangle(i,i+1,i+2);vh.AddTriangle(i,i+2,i+3);
        }
''' + s[b:];p.write_text(s,encoding='utf-8')
