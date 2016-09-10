namespace Ouroboros.Windows
{
    public static class Assets
    {
        public static class Fonts
        {
            private const string FontsPath = "Fonts/";

            public const string KenVectorFuture12 = FontsPath + nameof(KenVectorFuture12);

            public const string KenVectorFuture18 = FontsPath + nameof(KenVectorFuture18);

            public const string KenVectorFutureThin12 = FontsPath + nameof(KenVectorFutureThin12);

            public const string KenVectorFutureThin18 = FontsPath + nameof(KenVectorFutureThin18);
        }

        public static class Sprites
        {
            private const string SpritesPath = "Sprites/";

            public static class UserInterface
            {
                private const string UserInterfacePath = SpritesPath + "UserInterface/";

                public static class Panel
                {
                    private const string PanelPath = UserInterfacePath + "Panel/";

                    public const string GlassPanel = PanelPath + nameof(GlassPanel) + ".png";

                    public const string GlassPanelCorners = PanelPath + nameof(GlassPanelCorners) + ".png";

                    // GlassPanelTab is unused
                }
            }
        }
    }
}