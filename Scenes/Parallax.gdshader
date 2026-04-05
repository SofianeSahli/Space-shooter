shader_type canvas_item;

uniform vec2 target_resolution = vec2(1280.0, 720.0);
uniform vec2 scroll_speed = vec2(0.1, 0.05);  // Different speeds for parallax effect
uniform float zoom = 1.0;

void fragment()
{
    vec2 uv = UV;
    
    // Get texture dimensions
    vec2 tex_size = vec2(textureSize(TEXTURE, 0));
    vec2 target_size = target_resolution;
    
    // Calculate scale to cover entire screen
    vec2 scale = target_size / tex_size;
    float uniform_scale = max(scale.x, scale.y) * zoom;
    
    // Scale UVs from center
    uv = (uv - 0.5) * (1.0 / uniform_scale) + 0.5;
    
    // Apply scrolling for parallax
    uv += scroll_speed * TIME;
    
    // Wrap for seamless tiling (good for backgrounds)
    uv = fract(uv);
    
    COLOR = texture(TEXTURE, uv);
}