namespace VT100.FullScreen.ScreenLayers
{
    /// <summary>
    /// A request for a box character. Boxes can be drawn in multiple formats, and boxes that overlap will need to
    /// be merged with appropriate junction characters selected where required. This class describes the requirements
    /// of a single character. Character set differences will limit how well the requested character can be matched,
    /// but this request does not need to take this into account.   
    /// </summary>
    internal struct BoxCharRequest
    {
        public BoxCharacter Class { get; set; }

        public CornerType RequestedCornerType { get; set; }
        public Edge RequestedLeft { get; set; }
        public Edge RequestedRight { get; set; }
        public Edge RequestedUp { get; set; }
        public Edge RequestedDown { get; set; }
    }
}