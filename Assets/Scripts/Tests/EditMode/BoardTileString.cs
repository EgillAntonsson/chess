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

	public static string Check_but_king_can_move_but_not_castle()
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
";
	}

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

	public static string Can_castle_king_side()
	{
		return @"
R2 N2 B2 Q2 K2 B2 N2 R2
P2 P2 P2 -- -- -- P2 P2
-- -- -- -- -- P2 -- --
-- -- -- P2 P2 -- -- --
-- -- -- -- -- -- -- --
-- -- -- B1 P1 N1 -- --
P1 P1 P1 P1 -- P1 P1 P1
R1 N1 B1 Q1 K1 -- -- R1
";
	}

	public static string Can_castle_queen_side()
	{
		return @"
R2 N2 B2 Q2 K2 B2 N2 R2
-- -- -- -- -- -- P2 P2
P2 P2 P2 P2 P2 P2 -- --
-- -- -- -- -- -- -- --
-- -- P1 P1 P1 -- -- --
-- -- N1 -- B1 Q1 -- --
P1 P1 -- -- -- P1 P1 P1
R1 -- -- -- K1 B1 N1 R1
";
	}

	public static string Can_castle_on_both_sides()
	{
		return @"
R2 N2 B2 Q2 K2 B2 N2 R2
-- -- -- -- -- -- -- --
P2 P2 P2 P2 P2 P2 P2 P2
-- -- -- -- -- -- -- --
-- -- P1 P1 P1 -- -- --
-- -- N1 B1 B1 Q1 -- N1
P1 P1 -- -- -- P1 P1 P1
R1 -- -- -- K1 -- -- R1
";
	}

	public static string Can_not_castle_as_1st_intra_move_would_be_checked()
	{
		return @"
R2 N2 B2 Q2 K2 B2 N2 R2
-- -- -- -- -- -- P2 P2
P2 P2 P2 -- P2 P2 -- --
-- -- -- -- -- -- -- --
-- -- P1 -- P1 -- -- --
-- -- N1 -- B1 Q1 -- --
P1 P1 -- -- -- P1 P1 P1
R1 -- -- -- K1 B1 N1 R1
";
	}
	
	public static string Can_not_castle_as_2nd_intra_move_would_be_checked()
	{
		return @"
R2 N2 -- -- K2 B2 N2 R2
-- B2 Q2 -- -- -- P2 P2
P2 P2 -- P2 P2 P2 -- --
-- -- -- -- -- -- -- --
-- -- -- P1 P1 -- -- --
N1 -- -- -- B1 Q1 -- --
P1 P1 -- -- -- P1 P1 P1
R1 -- -- -- K1 B1 N1 R1
";
	}
}