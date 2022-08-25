namespace VT100.FullScreen.ScreenLayers
{
    /// <summary>
    /// Enumeration for box character types. Note that the values used are the unicode character values. The literal
    /// in the comment is the unicode character itself. 
    /// </summary>
    public enum BoxCharacterType
    {
        //Light and heavy solid lines
        LIGHT_HORIZONTAL = 0x2500, //	 ─ 	
        // 	 	=	Videotex Mosaic DG 15
        // 	 	→	1FBAF 🮯 box drawings light horizontal with vertical stroke
        HEAVY_HORIZONTAL = 0x2501, //	 ━ 	
        LIGHT_VERTICAL = 0x2502, //	 │ 	
        // 	 	=	Videotex Mosaic DG 14
        HEAVY_VERTICAL = 0x2503, //	 ┃ 	
        
        //Light and heavy dashed lines
        LIGHT_TRIPLE_DASH_HORIZONTAL = 0x2504, //	 ┄ 	
        HEAVY_TRIPLE_DASH_HORIZONTAL = 0x2505, //	 ┅ 	
        LIGHT_TRIPLE_DASH_VERTICAL = 0x2506, //	 ┆ 	
        HEAVY_TRIPLE_DASH_VERTICAL = 0x2507, //	 ┇ 	
        LIGHT_QUADRUPLE_DASH_HORIZONTAL = 0x2508, //	 ┈ 	
        HEAVY_QUADRUPLE_DASH_HORIZONTAL = 0x2509, //	 ┉ 	
        LIGHT_QUADRUPLE_DASH_VERTICAL = 0x250A, //	 ┊ 	
        HEAVY_QUADRUPLE_DASH_VERTICAL = 0x250B, //	 ┋ 	
        
        //Light and heavy line box components
        LIGHT_DOWN_AND_RIGHT = 0x250C, // ┌ 		 
        // 	 	=	Videotex Mosaic DG 16
        DOWN_LIGHT_AND_RIGHT_HEAVY = 0x250D, //	 ┍ 	
        DOWN_HEAVY_AND_RIGHT_LIGHT = 0x250E, //	 ┎ 	
        HEAVY_DOWN_AND_RIGHT = 0x250F, //	 ┏ 	
        LIGHT_DOWN_AND_LEFT = 0x2510, //	 ┐ 	
        // 	 	=	Videotex Mosaic DG 17
        DOWN_LIGHT_AND_LEFT_HEAVY = 0x2511, //	 ┑ 	
        DOWN_HEAVY_AND_LEFT_LIGHT = 0x2512, //	 ┒ 	
        HEAVY_DOWN_AND_LEFT = 0x2513, //	 ┓ 	
        LIGHT_UP_AND_RIGHT = 0x2514, //	 └ 	
        // 	 	=	Videotex Mosaic DG 18
        UP_LIGHT_AND_RIGHT_HEAVY = 0x2515, //	 ┕ 	
        UP_HEAVY_AND_RIGHT_LIGHT = 0x2516, //	 ┖ 	
        HEAVY_UP_AND_RIGHT = 0x2517, //	 ┗ 	
        LIGHT_UP_AND_LEFT = 0x2518, //	 ┘ 	
        // 	 	=	Videotex Mosaic DG 19
        UP_LIGHT_AND_LEFT_HEAVY = 0x2519, //	 ┙ 	
        UP_HEAVY_AND_LEFT_LIGHT = 0x251A, //	 ┚ 	
        HEAVY_UP_AND_LEFT = 0x251B, //	 ┛ 	
        LIGHT_VERTICAL_AND_RIGHT = 0x251C, //	 ├ 	
        // 	 	=	Videotex Mosaic DG 20
        VERTICAL_LIGHT_AND_RIGHT_HEAVY = 0x251D, //	 ┝ 	
        // 	 	=	Videotex Mosaic DG 03
        UP_HEAVY_AND_RIGHT_DOWN_LIGHT = 0x251E, //	 ┞ 	
        DOWN_HEAVY_AND_RIGHT_UP_LIGHT = 0x251F, //	 ┟ 	
        VERTICAL_HEAVY_AND_RIGHT_LIGHT = 0x2520, //	 ┠ 	
        DOWN_LIGHT_AND_RIGHT_UP_HEAVY = 0x2521, //	 ┡ 	
        UP_LIGHT_AND_RIGHT_DOWN_HEAVY = 0x2522, //	 ┢ 	
        HEAVY_VERTICAL_AND_RIGHT = 0x2523, //	 ┣ 	
        LIGHT_VERTICAL_AND_LEFT = 0x2524, //	 ┤ 	
        // 	 	=	Videotex Mosaic DG 21
        VERTICAL_LIGHT_AND_LEFT_HEAVY = 0x2525, //	 ┥ 	
        // 	 	=	Videotex Mosaic DG 04
        UP_HEAVY_AND_LEFT_DOWN_LIGHT = 0x2526, //	 ┦ 	
        DOWN_HEAVY_AND_LEFT_UP_LIGHT = 0x2527, //	 ┧ 	
        VERTICAL_HEAVY_AND_LEFT_LIGHT = 0x2528, //	 ┨ 	
        DOWN_LIGHT_AND_LEFT_UP_HEAVY = 0x2529, //	 ┩ 	
        UP_LIGHT_AND_LEFT_DOWN_HEAVY = 0x252A, //	 ┪ 	
        HEAVY_VERTICAL_AND_LEFT = 0x252B, //	 ┫ 	
        LIGHT_DOWN_AND_HORIZONTAL = 0x252C, //	 ┬ 	
        // 	 	=	Videotex Mosaic DG 22
        LEFT_HEAVY_AND_RIGHT_DOWN_LIGHT = 0x252D, //	 ┭ 	
        RIGHT_HEAVY_AND_LEFT_DOWN_LIGHT = 0x252E, //	 ┮ 	
        DOWN_LIGHT_AND_HORIZONTAL_HEAVY = 0x252F, //	 ┯ 	
        // 	 	=	Videotex Mosaic DG 02
        DOWN_HEAVY_AND_HORIZONTAL_LIGHT = 0x2530, //	 ┰ 	
        RIGHT_LIGHT_AND_LEFT_DOWN_HEAVY = 0x2531, //	 ┱ 	
        LEFT_LIGHT_AND_RIGHT_DOWN_HEAVY = 0x2532, //	 ┲ 	
        HEAVY_DOWN_AND_HORIZONTAL = 0x2533, //	 ┳ 	
        LIGHT_UP_AND_HORIZONTAL = 0x2534, //	 ┴ 	
        // 	 	=	Videotex Mosaic DG 23
        LEFT_HEAVY_AND_RIGHT_UP_LIGHT = 0x2535, //	 ┵ 	
        RIGHT_HEAVY_AND_LEFT_UP_LIGHT = 0x2536, //	 ┶ 	
        UP_LIGHT_AND_HORIZONTAL_HEAVY = 0x2537, //	 ┷ 	
        // 	 	=	Videotex Mosaic DG 01
        UP_HEAVY_AND_HORIZONTAL_LIGHT = 0x2538, //	 ┸ 	
        RIGHT_LIGHT_AND_LEFT_UP_HEAVY = 0x2539, //	 ┹ 	
        LEFT_LIGHT_AND_RIGHT_UP_HEAVY = 0x253A, //	 ┺ 	
        HEAVY_UP_AND_HORIZONTAL = 0x253B, //	 ┻ 	
        LIGHT_VERTICAL_AND_HORIZONTAL = 0x253C, //	 ┼ 	
        // 	 	=	Videotex Mosaic DG 24
        LEFT_HEAVY_AND_RIGHT_VERTICAL_LIGHT = 0x253D, //	 ┽ 	
        RIGHT_HEAVY_AND_LEFT_VERTICAL_LIGHT = 0x253E, //	 ┾ 	
        VERTICAL_LIGHT_AND_HORIZONTAL_HEAVY = 0x253F, //	 ┿ 	
        // 	 	=	Videotex Mosaic DG 13
        UP_HEAVY_AND_DOWN_HORIZONTAL_LIGHT = 0x2540, //	 ╀ 	
        DOWN_HEAVY_AND_UP_HORIZONTAL_LIGHT = 0x2541, //	 ╁ 	
        VERTICAL_HEAVY_AND_HORIZONTAL_LIGHT = 0x2542, //	 ╂ 	
        LEFT_UP_HEAVY_AND_RIGHT_DOWN_LIGHT = 0x2543, //	 ╃ 	
        RIGHT_UP_HEAVY_AND_LEFT_DOWN_LIGHT = 0x2544, //	 ╄ 	
        LEFT_DOWN_HEAVY_AND_RIGHT_UP_LIGHT = 0x2545, //	 ╅ 	
        RIGHT_DOWN_HEAVY_AND_LEFT_UP_LIGHT = 0x2546, //	 ╆ 	
        DOWN_LIGHT_AND_UP_HORIZONTAL_HEAVY = 0x2547, //	 ╇ 	
        UP_LIGHT_AND_DOWN_HORIZONTAL_HEAVY = 0x2548, //	 ╈ 	
        RIGHT_LIGHT_AND_LEFT_VERTICAL_HEAVY = 0x2549, //	 ╉ 	
        LEFT_LIGHT_AND_RIGHT_VERTICAL_HEAVY = 0x254A, //	 ╊ 	
        HEAVY_VERTICAL_AND_HORIZONTAL = 0x254B, //	 ╋ 	

        //Light and heavy dashed lines
        LIGHT_DOUBLE_DASH_HORIZONTAL = 0x254C, //	 ╌ 	
        HEAVY_DOUBLE_DASH_HORIZONTAL = 0x254D, //	 ╍ 	
        LIGHT_DOUBLE_DASH_VERTICAL = 0x254E, //	 ╎ 	
        HEAVY_DOUBLE_DASH_VERTICAL = 0x254F, //	 ╏ 	

        //Double lines
        DOUBLE_HORIZONTAL = 0x2550, //	 ═ 	
        DOUBLE_VERTICAL = 0x2551, //	 ║ 	

        //Light and double line box components
        DOWN_SINGLE_AND_RIGHT_DOUBLE = 0x2552, //	 ╒ 	
        DOWN_DOUBLE_AND_RIGHT_SINGLE = 0x2553, //	 ╓ 	
        DOUBLE_DOWN_AND_RIGHT = 0x2554, //	 ╔ 	
        DOWN_SINGLE_AND_LEFT_DOUBLE = 0x2555, //	 ╕ 	
        DOWN_DOUBLE_AND_LEFT_SINGLE = 0x2556, //	 ╖ 	
        DOUBLE_DOWN_AND_LEFT = 0x2557, //	 ╗ 	
        UP_SINGLE_AND_RIGHT_DOUBLE = 0x2558, //	 ╘ 	
        UP_DOUBLE_AND_RIGHT_SINGLE = 0x2559, //	 ╙ 	
        DOUBLE_UP_AND_RIGHT = 0x255A, //	 ╚ 	
        UP_SINGLE_AND_LEFT_DOUBLE = 0x255B, //	 ╛ 	
        UP_DOUBLE_AND_LEFT_SINGLE = 0x255C, //	 ╜ 	
        DOUBLE_UP_AND_LEFT = 0x255D, //	 ╝ 	
        VERTICAL_SINGLE_AND_RIGHT_DOUBLE = 0x255E, //	 ╞ 	
        VERTICAL_DOUBLE_AND_RIGHT_SINGLE = 0x255F, //	 ╟ 	
        DOUBLE_VERTICAL_AND_RIGHT = 0x2560, //	 ╠ 	
        VERTICAL_SINGLE_AND_LEFT_DOUBLE = 0x2561, //	 ╡ 	
        VERTICAL_DOUBLE_AND_LEFT_SINGLE = 0x2562, //	 ╢ 	
        DOUBLE_VERTICAL_AND_LEFT = 0x2563, //	 ╣ 	
        DOWN_SINGLE_AND_HORIZONTAL_DOUBLE = 0x2564, //	 ╤ 	
        DOWN_DOUBLE_AND_HORIZONTAL_SINGLE = 0x2565, //	 ╥ 	
        DOUBLE_DOWN_AND_HORIZONTAL = 0x2566, //	 ╦ 	
        UP_SINGLE_AND_HORIZONTAL_DOUBLE = 0x2567, //	 ╧ 	
        UP_DOUBLE_AND_HORIZONTAL_SINGLE = 0x2568, //	 ╨ 	
        DOUBLE_UP_AND_HORIZONTAL = 0x2569, //	 ╩ 	
        VERTICAL_SINGLE_AND_HORIZONTAL_DOUBLE = 0x256A, //	 ╪ 	
        VERTICAL_DOUBLE_AND_HORIZONTAL_SINGLE = 0x256B, //	 ╫ 	
        DOUBLE_VERTICAL_AND_HORIZONTAL = 0x256C, //	 ╬ 	

        //Character cell arcs
        LIGHT_ARC_DOWN_AND_RIGHT = 0x256D, //	 ╭ 	
        LIGHT_ARC_DOWN_AND_LEFT = 0x256E, //	 ╮ 	
        LIGHT_ARC_UP_AND_LEFT = 0x256F, //	 ╯ 	
        LIGHT_ARC_UP_AND_RIGHT = 0x2570, //	 ╰ 	

        //Character cell diagonals
        LIGHT_DIAGONAL_UPPER_RIGHT_TO_LOWER_LEFT = 0x2571, //	 ╱ 	
        LIGHT_DIAGONAL_UPPER_LEFT_TO_LOWER_RIGHT = 0x2572, //	 ╲ 	
        LIGHT_DIAGONAL_CROSS = 0x2573, //	 ╳ 	
        
        //Light and heavy half lines
        LIGHT_LEFT = 0x2574, //	 ╴ 	
        LIGHT_UP = 0x2575, //	 ╵ 	
        LIGHT_RIGHT = 0x2576, //	 ╶ 	
        LIGHT_DOWN = 0x2577, //	 ╷ 	
        HEAVY_LEFT = 0x2578, //	 ╸ 	
        HEAVY_UP = 0x2579, //	 ╹ 	
        HEAVY_RIGHT = 0x257A, //	 ╺ 	
        HEAVY_DOWN = 0x257B, //	 ╻ 	

        //Mixed light and heavy lines
        LIGHT_LEFT_AND_HEAVY_RIGHT = 0x257C, //	 ╼ 	
        LIGHT_UP_AND_HEAVY_DOWN = 0x257D, //	 ╽ 	
        HEAVY_LEFT_AND_LIGHT_RIGHT = 0x257E, //	 ╾ 	
        HEAVY_UP_AND_LIGHT_DOWN = 0x257F, //	 ╿ 	
    }
}