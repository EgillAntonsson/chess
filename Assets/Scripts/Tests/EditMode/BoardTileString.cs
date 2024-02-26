/// <summary>
/// Notation names are 1 based.
/// </summary>
public static class BoardTileString 
{
		public static string Notation_1_e4()
		{
			return @"
R2 N2 B2 Q2 K2 B2 N2 R2
P2 P2 P2 P2 P2 P2 P2 P2
-- -- -- -- -- -- -- --
-- -- -- -- -- -- -- --
-- -- -- -- P1 -- -- --
-- -- -- -- -- -- -- --
P1 P1 P1 P1 -- P1 P1 P1
R1 N1 B1 Q1 K1 B1 N1 R1
";
		}
	
		public static string Notation_1_e4_c5()
		{
			return @"
R2 N2 B2 Q2 K2 B2 N2 R2
P2 P2 -- P2 P2 P2 P2 P2
-- -- -- -- -- -- -- --
-- -- P2 -- -- -- -- --
-- -- -- -- P1 -- -- --
-- -- -- -- -- -- -- --
P1 P1 P1 P1 -- P1 P1 P1
R1 N1 B1 Q1 K1 B1 N1 R1
";
		}

		public static string Notation_1_e4_e5()
		{
			return @"
R2 N2 B2 Q2 K2 B2 N2 R2
P2 P2 P2 P2 -- P2 P2 P2
-- -- -- -- -- -- -- --
-- -- -- -- P2 -- -- --
-- -- -- -- P1 -- -- --
-- -- -- -- -- -- -- --
P1 P1 P1 P1 -- P1 P1 P1
R1 N1 B1 Q1 K1 B1 N1 R1
";
		}
		public static string Check_but_piece_can_defend()
		{
			return @"
R2 N2 B2 -- K2 B2 N2 R2
P2 P2 P2 -- Q2 P2 P2 P2
-- -- -- -- -- -- -- --
-- -- -- P1 -- -- -- --
-- -- -- -- -- -- -- --
-- -- -- -- -- -- -- --
P1 P1 P1 P1 -- P1 P1 P1
R1 N1 B1 Q1 K1 B1 N1 R1
";
		}

		public static string Check_but_king_can_move()
		{
			return @"
R2 N2 B2 -- K2 B2 N2 R2
P2 P2 -- -- Q2 -- -- P2
-- -- P2 -- -- P2 -- --
-- -- -- P2 -- -- P2 --
-- -- P1 -- -- -- P1 --
-- Q1 -- P1 -- P1 -- N1
P1 P1 -- -- -- -- B1 P1
R1 N1 -- -- K1 -- -- R1
"; }

		public static string Quickest_Win__Player2_To_Move()
		{
			return @"
R2 N2 B2 Q2 K2 B2 N2 R2
P2 P2 P2 P2 -- P2 P2 P2
-- -- -- -- -- -- -- --
-- -- -- -- P2 -- -- --
-- -- -- -- -- -- P1 --
-- -- -- -- -- P1 -- --
P1 P1 P1 P1 P1 -- -- P1
R1 N1 B1 Q1 K1 B1 N1 R1
";
		}

		public static string Player1_CheckMate()
		{
			return @"
R2 N2 B2 -- K2 B2 N2 R2
P2 P2 P2 P2 -- P2 P2 P2
-- -- -- -- -- -- -- --
-- -- -- -- P2 -- -- --
-- -- -- -- -- -- P1 Q2
-- -- -- -- -- P1 -- --
P1 P1 P1 P1 P1 -- -- P1
R1 N1 B1 Q1 K1 B1 N1 R1
";
		}
}


// return @"
// R2 N2 B2 -- K2 B2 N2 R2
// P2 P2 -- -- Q2 P2 P2 P2
// -- -- P2 -- -- -- -- --
// -- -- -- P1 -- -- -- --
// -- -- -- -- -- P1 -- --
// -- -- -- -- -- -- -- --
// P1 P1 P1 P1 -- -- P1 P1
// R1 N1 B1 Q1 K1 B1 N1 R1
// "; }