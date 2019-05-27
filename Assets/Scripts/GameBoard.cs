using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameBoard {

	private int size = 0;
	public int Size {
		get { return size; }
	}
	private bool[,] board;
	public bool this[int x, int y] {
		get {
			return board[x, y];
		}
	}

	public GameBoard(int size) {
		this.size = size;
		this.board = new bool[size, size];
	}

	public List<Tuple<int, int>> ExecuteStep() {
		List<Tuple<int, int>> changes = new List<Tuple<int, int>>();

		for (int x = 0; x < size; x++) {
			for (int y = 0; y < size; y++) {
				int neighbours = CountNeighbours(x, y);

				if (board[x, y]) {
					if (neighbours != 2 && neighbours != 3) {
						changes.Add(new Tuple<int, int>(x, y));
					}
				} else if (neighbours == 3) {
					changes.Add(new Tuple<int, int>(x, y));
				}
			}
		}

		foreach (Tuple<int, int> change in changes) {
			board[change.Item1, change.Item2] = !board[change.Item1, change.Item2];
		}

		return changes;
	}

	public List<Tuple<int, int>> ClearBoard() {
		List<Tuple<int, int>> changes = new List<Tuple<int, int>>();
		for (int x = 0; x < size; x++) {
			for (int y = 0; y < size; y++) {
				if (board[x, y]) {
					board[x, y] = false;
					changes.Add(new Tuple<int, int>(x, y));
				}
			}
		}

		return changes;
	}

	private int CountNeighbours(int x, int y) {
		int neighbours = 0;
		for (int neighbourX = -1; neighbourX <= 1; neighbourX++) {
			for (int neighbourY = -1; neighbourY <= 1; neighbourY++) {
				if (neighbourX == 0 && neighbourY == 0) continue;

				int pointX = x + neighbourX;
				int pointY = y + neighbourY;
				if (IsValidPoint(pointX, pointY) && board[pointX, pointY]) {
					neighbours++;
				}
			}
		}

		return neighbours;
	}

	public bool IsValidPoint(int x, int y) {
		return x >= 0 && x < size && y >= 0 && y < size;
	}

	public bool TogglePoint(int x, int y) {
		board[x, y] = !board[x, y];
		return board[x, y];
	}

}
