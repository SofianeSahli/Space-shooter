shader_type canvas_item;

uniform float value = 0.7;        // 0 → 1 (current speed)
uniform float max_speed_value = 1.0;
uniform float radius = 0.7;
uniform float thickness = 0.1;

uniform float bar_count = 60.0;   // more bars for full circle
uniform float bar_width = 0.08;

uniform float pulse_speed = 1.0;
uniform float glow_intensity = 0;

// Visual effects
uniform bool show_needle = false;
uniform bool show_text = false;

// Colors
uniform vec3 color_low = vec3(0.2, 1.0, 0.3);    // green
uniform vec3 color_mid = vec3(1.0, 0.8, 0.2);    // yellow  
uniform vec3 color_high = vec3(1.0, 0.2, 0.2);   // red

void fragment()
{
    vec2 uv = UV - 0.5;
    float dist = length(uv);
    
    // =========================
    // 🟢 FULL CIRCLE (no restriction)
    // =========================
    if (dist > radius)
        discard;
    
    // =========================
    // 🧭 ANGLE → 0..1 (full circle)
    // =========================
    float angle = atan(uv.y, uv.x);        // -PI → PI
    float angle01 = (angle + PI) / (2.0 * PI);  // 0 → 1 (full circle)
    
    // =========================
    // 📊 BAR INDEX
    // =========================
    float barIndex = floor(angle01 * bar_count);
    float barPos = barIndex / bar_count;
    
    // bar mask (creates gaps)
    float localAngle = fract(angle01 * bar_count);
    float barMask = step(localAngle, bar_width);
    
    // =========================
    // 🟢 RING SHAPE
    // =========================
    float ring = smoothstep(radius, radius - thickness, dist);
    
    // =========================
    // 🔥 ACTIVE BARS (fills from bottom/right, empties counter-clockwise)
    // =========================
    // For a speedometer that fills clockwise from bottom
    float active = 0.0;
    
    // Option 1: Fill clockwise from bottom (like traditional speedometer)
    float zeroAngle = 0.0;  // bottom (or wherever you want zero)
    float fillAngle = value * 2.0 * PI;
    
    // Check if current angle is within filled portion
    float angleRad = angle + PI;  // 0 to 2PI
    if (angleRad <= fillAngle) {
        active = 1.0;
    }
    
    // Alternative: Fill from any starting point
    // float startAngle = 0.0;  // 0 = right, PI/2 = top, PI = left, 3*PI/2 = bottom
    // float angleFromStart = mod(angleRad - startAngle + 2.0*PI, 2.0*PI);
    // active = step(angleFromStart, fillAngle);
    
    // Pulsing effect near current speed
    float pulseZone = 0.0;
    float angleDiff = abs(angleRad - fillAngle);
    if (angleDiff < 0.2) {
        pulseZone = 1.0 - (angleDiff / 0.2);
    }
    float pulse = 0.8 + sin(TIME * pulse_speed) * 0.3;
    
    // Glow effect at current speed position
    float glowZone = exp(-pow((angleRad - fillAngle) * 8.0, 2.0));
    
    float final = ring * barMask * active;
    
    // =========================
    // 🎨 COLOR GRADIENT (based on fill percentage)
    // =========================
    vec3 color;
    float fillPercent = barPos;
    if (fillPercent < 0.5) {
        color = mix(color_low, color_mid, fillPercent * 2.0);
    } else {
        color = mix(color_mid, color_high, (fillPercent - 0.5) * 2.0);
    }
    
    // Add pulse effect to active bars near current speed
    color *= (1.0 + pulseZone * pulse * 0.5);
    
    // Add glow
    color += vec3(1.0, 1.0, 1.0) * glowZone * glow_intensity;
    
    COLOR = vec4(color * final, final);
    
    // =========================
    // 🪡 NEEDLE
    // =========================
    if (show_needle) {
        float needleAngle = value * 2.0 * PI - PI;  // Convert to -PI to PI range
        vec2 needleDir = vec2(cos(needleAngle), sin(needleAngle));
        
        // Needle thickness
        float needleThick = 0.015;
        float needleLength = radius - thickness * 0.5;
        
        // Check if pixel is near needle line
        float needleDist = abs(uv.x * needleDir.y - uv.y * needleDir.x);
        float inNeedle = step(needleDist, needleThick) * step(length(uv), needleLength);
        
        // Needle center dot
        float centerDist = length(uv);
        float centerDot = step(centerDist, 0.05);
        
        // Needle color with glow
        vec3 needleColor = vec3(1.0, 0.5, 0.2);
        float needlePulse = 0.7 + sin(TIME * 8.0) * 0.3;
        
        COLOR.rgb = mix(COLOR.rgb, needleColor * needlePulse, inNeedle);
        COLOR.rgb = mix(COLOR.rgb, vec3(1.0), centerDot * 0.8);
        COLOR.a = max(COLOR.a, inNeedle * 0.9);
        COLOR.a = max(COLOR.a, centerDot * 0.8);
    }
    
    // =========================
    // 🔘 TICK MARKS (every 10%)
    // =========================
    for (int i = 0; i <= 10; i++) {
        float markPercent = float(i) / 10.0;
        float markAngle = markPercent * 2.0 * PI - PI;
        vec2 markDir = vec2(cos(markAngle), sin(markAngle));
        
        float markLength = (i % 2 == 0) ? 0.03 : 0.02;  // longer marks every 20%
        float markDist = abs(uv.x * markDir.y - uv.y * markDir.x);
        float mark = step(markDist, 0.01) * 
                     step(radius - thickness - markLength, length(uv)) * 
                     step(length(uv), radius - thickness);
        
        vec3 markColor = (i % 2 == 0) ? vec3(1.0) : vec3(0.7);
        COLOR.rgb = mix(COLOR.rgb, markColor, mark);
        COLOR.a = max(COLOR.a, mark);
    }
    
    // =========================
    // 💫 SPEED PARTICLES (moving dots showing flow)
    // =========================
    float particleCount = 12.0;
    for(float i = 0.0; i < particleCount; i++) {
        // Particles move ahead of the current speed
        float particleOffset = fract(TIME * 1.5 + i / particleCount);
        float particleAngle = (value + particleOffset * 0.3) * 2.0 * PI - PI;
        particleAngle = mod(particleAngle + 2.0*PI, 2.0*PI) - PI;
        
        float particleRadius = radius - thickness * 0.6;
        vec2 particlePos = vec2(cos(particleAngle), sin(particleAngle)) * particleRadius;
        
        float particleDist = length(uv - particlePos);
        float particle = step(particleDist, 0.015);
        
        vec3 particleColor = vec3(1.0, 0.8, 0.2);
        COLOR.rgb = mix(COLOR.rgb, particleColor, particle * 0.7);
        COLOR.a = max(COLOR.a, particle * 0.9);
    }
    
    // =========================
    // 📝 SPEED TEXT
    // =========================
    if (show_text) {
        float speedPercent = value / max_speed_value;
        int speedValue = int(speedPercent * 100.0);
        
        // Create a simple digital readout in the center
        vec2 textUV = uv;
        
        // Center circle background
        float centerBg = step(length(uv), 0.2);
        COLOR.rgb = mix(COLOR.rgb, vec3(0.05, 0.05, 0.1), centerBg * 0.8);
        
        // Draw a simple circular background for the text
        if (length(uv) < 0.18) {
            // This would be where you'd render actual text
            // For now, we'll draw a simple indicator
            
            // Draw percentage bar in center
            float barHeight = 0.1;
            float barY = uv.y + 0.1;
            if (abs(uv.x) < 0.1 && barY > -0.1 && barY < 0.1) {
                float barFill = step(barY, -0.1 + speedPercent * 0.2);
                COLOR.rgb = mix(COLOR.rgb, color_high, barFill * 0.8);
            }
            
            COLOR.a = max(COLOR.a, centerBg * 0.5);
        }
    }
    
    // =========================
    // 💥 OVERSPEED WARNING
    // =========================
    if (value > max_speed_value) {
        float warning = sin(TIME * 15.0) * 0.5 + 0.5;
        COLOR.rgb += vec3(1.0, 0.2, 0.2) * warning * 0.5;
        
        // Alternating red flash on the entire gauge
        if (fract(TIME * 3.0) > 0.5) {
            COLOR.rgb = mix(COLOR.rgb, vec3(1.0, 0.0, 0.0), 0.2);
        }
    }
    
    // =========================
    // ✨ INNER GLOW
    // =========================
    float innerGlow = (1.0 - smoothstep(0.0, radius - thickness, dist)) * 0.3;
    COLOR.rgb += vec3(0.5, 0.8, 1.0) * innerGlow * (1.0 - value);
}