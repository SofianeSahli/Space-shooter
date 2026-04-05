shader_type canvas_item;

uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;

uniform float strength = 0.02;

void fragment()
{
    vec2 uv = SCREEN_UV;

    float mask = smoothstep(0.6, 0.0, distance(UV, vec2(0.5)));

    uv.x += sin(uv.y * 20.0 + TIME * 10.0) * strength * mask;

    vec3 col = texture(SCREEN_TEXTURE, uv).rgb;

    COLOR = vec4(col, 1.0);
}