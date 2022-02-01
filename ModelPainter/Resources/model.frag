#version 330 core

uniform vec3 lightPos;
uniform sampler2D texModel;
uniform int objectIdMode;
uniform uint selectedCuboidId;

in vec3 fragPos;
in vec3 fragNormal;
in vec2 fragTexCoord;
flat in vec4 fragObjectId;

#define ObjectIdModeDisabled 0
#define ObjectIdModeCubeTest 1
#define ObjectIdModeAlphaTest 2
#define ObjectIdModeTexCoord 3

out vec4 color;

void main()
{
    // TODO: remove branching
    if (objectIdMode == ObjectIdModeCubeTest)
    {
        color = vec4(fragObjectId.rgb, 1.0);
        return;
    }

    if (objectIdMode == ObjectIdModeTexCoord)
    {
        color = vec4(vec3(fragTexCoord, 0.0), 1.0);
        return;
    }

    vec4 samp = texture(texModel, fragTexCoord);

    if (samp.a < 1)
    {
        discard;
    }

    if (objectIdMode == ObjectIdModeDisabled)
    {
        vec3 norm = normalize(fragNormal);
        vec3 lightDir = normalize(lightPos);// light very far away, use direction only. If light has position: normalize(lightPos - fragPos)  
        float diffuse = clamp(dot(norm, lightDir), -1, 1) * 0.3;
        float ambient = 0.7;

        uint objectId = uint(fragObjectId.b * 16581375 + fragObjectId.g * 65025 + fragObjectId.r * 255);

        samp = vec4(samp.rgb * clamp(ambient + diffuse, 0, 1), 1.0);

        if (objectId == selectedCuboidId)
        {
            color = mix(samp, vec4(1.0, 1.0, 1.0, 1.0), 0.5);
        }
        else
        {
            color = samp;
        }

    }

    if (objectIdMode == ObjectIdModeAlphaTest)
    {
        color = vec4(fragObjectId.rgb, 1.0);
    }
}