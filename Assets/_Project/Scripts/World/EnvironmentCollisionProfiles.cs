using UnityEngine;

public static class EnvironmentCollisionProfiles
{
    public readonly struct Shape
    {
        public readonly Vector3 Center;
        public readonly Vector3 Size;

        public Shape(Vector3 center, Vector3 size)
        {
            Center = center;
            Size = size;
        }
    }

    private static readonly Shape[] Barrier004 = One(0f, 0.68f, 0f, 1.4f, 1.4f, 1.4f);
    private static readonly Shape[] Box003 = One(0f, 0.42f, 0f, 1.52f, 0.85f, 1.03f);
    private static readonly Shape[] Barrel005 = One(0f, 0.45f, 0f, 0.68f, 0.9f, 0.68f);
    private static readonly Shape[] Tires001 = One(0f, 0.52f, 0f, 1.1f, 1.03f, 1.1f);
    private static readonly Shape[] Tent002 = One(-0.01f, 1.75f, -0.46f, 7.45f, 3.7f, 9.9f);
    private static readonly Shape[] Generator004 = One(-0.02f, 0.93f, 0.02f, 1.95f, 1.85f, 3.2f);
    private static readonly Shape[] RadioStation001 = One(0f, 3.37f, 0.63f, 3.9f, 6.75f, 8f);
    private static readonly Shape[] Tower003 = One(0f, 6.16f, 0f, 4.8f, 12.33f, 4.7f);
    private static readonly Shape[] Hummer003 =
    {
        new Shape(new Vector3(0f, 0.95f, -0.35f), new Vector3(2.95f, 1.9f, 3.65f)),
        new Shape(new Vector3(0f, 0.68f, 1.55f), new Vector3(2.8f, 1.35f, 1.55f))
    };

    private static readonly Shape[] FortifiedWall1B = One(0f, 0.39f, 0f, 4.28f, 10.01f, 8.56f);
    private static readonly Shape[] FortifiedWall1C = One(0f, 0.44f, 0.49f, 4.28f, 10.01f, 9.1f);
    private static readonly Shape[] FortifiedWall1D = One(0f, 0.47f, 0f, 4.28f, 10.01f, 9.63f);
    private static readonly Shape[] FortifiedWall2A = One(0f, 0.3f, 0f, 2.68f, 10.01f, 17.12f);
    private static readonly Shape[] FortifiedWall2B = One(0f, 0.34f, 0f, 4.28f, 10.01f, 17.12f);
    private static readonly Shape[] FortifiedWall2C = One(0f, 0.38f, -0.69f, 4.28f, 10.01f, 17.66f);
    private static readonly Shape[] FortifiedWall2D = One(0f, 0.41f, 0f, 4.28f, 10.01f, 18.19f);

    private static readonly Shape[] WallDoor = One(0f, -0.38f, 0f, 0.76f, 4f, 4.28f);
    private static readonly Shape[] WallWindow1 = One(0f, -0.08f, 0f, 0.76f, 4f, 4.28f);
    private static readonly Shape[] WallWindow2 = One(0f, -0.07f, 0f, 0.76f, 4f, 4.28f);
    private static readonly Shape[] WallBroken1A = One(0f, -0.23f, 0.05f, 0.55f, 4f, 4.28f);
    private static readonly Shape[] WallBroken1B = One(0f, -0.17f, 0.05f, 0.55f, 2.55f, 4.28f);
    private static readonly Shape[] WallBroken1C = One(0f, -0.34f, 0.48f, 0.55f, 2.94f, 3.71f);

    private static readonly Shape[] ConcreteBlock1B = One(0f, 0f, 0f, 2.16f, 4f, 4.32f);
    private static readonly Shape[] ConcreteBlock1C = One(0f, 0f, 0f, 2.16f, 4f, 8.64f);
    private static readonly Shape[] ConcreteBlock1D = One(0f, 0f, 0f, 4.32f, 4f, 4.32f);
    private static readonly Shape[] ConcreteBlock1E = One(0f, 0f, 0f, 4.32f, 4f, 8.64f);
    private static readonly Shape[] ConcreteBlock1F = One(0f, 0f, 0f, 8.64f, 4f, 8.64f);

    private static readonly Shape[] Barrier1A = One(0.29f, 0.19f, -0.06f, 1.33f, 2.62f, 5.18f);
    private static readonly Shape[] Barrier1B = One(0.3f, 0.18f, 0f, 1.52f, 3.38f, 5.43f);
    private static readonly Shape[] Barrier1C = One(0.47f, 0.44f, -0.09f, 2.2f, 3.27f, 5.92f);
    private static readonly Shape[] ShantyBoard1A = One(0f, -0.01f, 0f, 0.18f, 3.46f, 3.82f);
    private static readonly Shape[] ShantyBoard1B = One(0f, 0f, 0f, 0.18f, 4f, 6.48f);
    private static readonly Shape[] Tapestry = One(-0.03f, -1.97f, -0.01f, 0.18f, 4.12f, 5.39f);
    private static readonly Shape[] Awning2A = Awning(0.19f);
    private static readonly Shape[] Awning2B = Awning(0.03f);

    public static Shape[] Get(string prefabName)
    {
        switch (prefabName)
        {
            case "Barrier_004": return Barrier004;
            case "Box_003": return Box003;
            case "Barrel_005": return Barrel005;
            case "Tires_001": return Tires001;
            case "Tent_002": return Tent002;
            case "Generator_004": return Generator004;
            case "Radiostation_001": return RadioStation001;
            case "Tower_003": return Tower003;
            case "Hummer_003": return Hummer003;
            case "Fortified_Wall_1B": return FortifiedWall1B;
            case "Fortified_Wall_1C": return FortifiedWall1C;
            case "Fortified_Wall_1D": return FortifiedWall1D;
            case "Fortified_Wall_2A": return FortifiedWall2A;
            case "Fortified_Wall_2B": return FortifiedWall2B;
            case "Fortified_Wall_2C": return FortifiedWall2C;
            case "Fortified_Wall_2D": return FortifiedWall2D;
            case "Wall_1A_Door": return WallDoor;
            case "Wall_1A_Window_1": return WallWindow1;
            case "Wall_1A_Window_2": return WallWindow2;
            case "Wall_Broken_1A": return WallBroken1A;
            case "Wall_Broken_1B": return WallBroken1B;
            case "Wall_Broken_1C": return WallBroken1C;
            case "Concrete_Block_1B": return ConcreteBlock1B;
            case "Concrete_Block_1C": return ConcreteBlock1C;
            case "Concrete_Block_1D": return ConcreteBlock1D;
            case "Concrete_Block_1E": return ConcreteBlock1E;
            case "Concrete_Block_1F": return ConcreteBlock1F;
            case "Barrier_1A": return Barrier1A;
            case "Barrier_1B": return Barrier1B;
            case "Barrier_1C": return Barrier1C;
            case "Awning_2A": return Awning2A;
            case "Awning_2B": return Awning2B;
            case "Shanty_Wall_Board_1A": return ShantyBoard1A;
            case "Shanty_Wall_Board_1B": return ShantyBoard1B;
            case "Hanging_Tapestry_1A": return Tapestry;
            case "Hanging_Tapestry_1B": return Tapestry;
            default: return null;
        }
    }

    public static bool IsLarge(Shape[] shapes)
    {
        for (int index = 0; index < shapes.Length; index++)
            if (shapes[index].Size.x >= 4f || shapes[index].Size.z >= 7f)
                return true;
        return false;
    }

    private static Shape[] One(float centerX, float centerY, float centerZ, float sizeX, float sizeY, float sizeZ)
    {
        return new[] { new Shape(new Vector3(centerX, centerY, centerZ), new Vector3(sizeX, sizeY, sizeZ)) };
    }

    private static Shape[] Awning(float centerX)
    {
        return new[]
        {
            new Shape(new Vector3(centerX, 0.45f, -0.02f), new Vector3(5.45f, 0.5f, 5.35f)),
            new Shape(new Vector3(centerX - 2.2f, -1.45f, -0.02f), new Vector3(0.38f, 3.6f, 4.8f)),
            new Shape(new Vector3(centerX + 2.2f, -1.45f, -0.02f), new Vector3(0.38f, 3.6f, 4.8f))
        };
    }
}
