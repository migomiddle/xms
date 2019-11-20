namespace Xms.Form.Abstractions.Component
{
    public sealed class CellLabelSettings
    {
        public CellLabelAlignment Alignment { get; set; } = CellLabelAlignment.Left;

        public CellLabelPosition Position { get; set; } = CellLabelPosition.Left;

        public string Color { get; set; }
        public string FontSize { get; set; }
        public string FontWeight { get; set; }
        public string BackgroundColor { get; set; }

        public int Width { get; set; } = 115;
    }
}