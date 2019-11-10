#iChannel0 "file://./__slider_inner.png"

const float border_width = .01;
const vec4 border_color = vec4(1);

void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
    vec2 uv = fragCoord/iResolution.xy;
    vec4 color = texture(iChannel0, uv);

    vec4 tcol = texture(iChannel0, uv);
    
    if (tcol.a == 0.0)
    {
        if (texture(iChannel0, uv + vec2(0.0,            border_width)).a  != 0.0 ||
            texture(iChannel0, uv + vec2(0.0,           -border_width)).a  != 0.0 ||
            texture(iChannel0, uv + vec2(border_width,   0.0)).a           != 0.0 ||
            texture(iChannel0, uv + vec2(-border_width,  0.0)).a           != 0.0 ||
            texture(iChannel0, uv + vec2(-border_width,  border_width)).a  != 0.0 ||
            texture(iChannel0, uv + vec2(-border_width, -border_width)).a  != 0.0 ||
            texture(iChannel0, uv + vec2(border_width,   border_width)).a  != 0.0 ||
            texture(iChannel0, uv + vec2(border_width,  -border_width)).a  != 0.0) 
        {
            tcol = border_color;
        }
    }

    fragColor = tcol;
}

