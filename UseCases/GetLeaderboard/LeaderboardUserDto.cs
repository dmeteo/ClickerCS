﻿using CSharpClicker.Domain;

namespace CSharpClicker.UseCases.GetLeaderboard;

public class LeaderboardUserDto
{
	public string UserName { get; init; }

	public long RecordScore { get; init; }

	public byte[] Avatar { get; init; } = [];
}
