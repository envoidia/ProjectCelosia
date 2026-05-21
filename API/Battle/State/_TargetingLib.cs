using System;
using System.Collections.Generic;
using API.Graphics;
using API.Input;
using API.Menu;
using API.Menu.State;
using API.Save;
using API.Util;
using Microsoft.Xna.Framework;

namespace API.Battle.State;

// Significant using order
using static API.Battle.State.BattleLib;
using static API.Input.InputPrompts;

internal static class _TargetingLib
{
    private static readonly ABlank _Reticle = new(_DrawReticle, RenderPriority.B1High)
    {
        Position = new(_UninitializedReticlePos),
        AnimType = AnimType.Custom
    };

    internal static readonly Menu.Menu _Menu = new("Targeting", _Reticle)
    {
        OnCreate = static () =>
        {
            _Reticle.Prog = Progress.Zero;
            Stage.Remove(_Reticle);
            _UpdateReticle();
        },
        OnDestroy = static () => _Reticle.Prog = Progress.One,
        OnUpdate = _Update,
        GetInputPrompt = static () =>
            Menu.State.State.GetInputPromptString(Move, Confirm, Back, Log, Inspect)
    };

    /// <summary>
    /// How many extra actions have been used for the currently acting Unit
    /// </summary>
    private static int _extraActions = 0;

    /// <summary>
    /// Indicates that the targeting input check has has found confirmation input
    /// </summary>
    private const int _TargetingConfirm = -1;

    private static void _Update(GameTime gt)
    {
        _CheckOpenLogInspect();

        if (InputLib.Check(Keybinds.Back))
        {
            _Moves[_selectingMove].Text = "";

            _SkillList.IsVisible = true;
            _SkillDesc.IsVisible = true;

            States.Battle.RemoveMenu();
            return;
        }

        // todo move to InputWidget
        int newIndex = _CheckInputTargeting(_indexTarget, _selectingMove,
            _selectedSkillInstance.Skill.Range);

        if (newIndex != _indexTarget && newIndex != _TargetingConfirm)
        {
            _indexTarget = newIndex;
            _UpdateReticle();
        }

        if (newIndex != _TargetingConfirm)
        {
            return;
        }

        Unit self = Battle.PlayerTeam.Units[_selectingMove];

        // todo Battle.GetUnitAtPos
        Unit target = _indexTarget < PosLib.LowestOpp
            ? Battle.PlayerTeam.Units[_indexTarget]
            : Battle.OpponentTeam.Units[_indexTarget - PosLib.LowestOpp];

        _CurMoves.Add(new Move(_selectedSkillInstance, self, target.Pos));
        // todo support ExA
        _Moves[_selectingMove].Text = $"{_Moves[_selectingMove].Text} /c[white]→ {target.FormatName(false)}";

        // Move on to next Unit unless this one has extra actions
        if (_extraActions < self.ExtraActions)
        {
            _extraActions++;
        }
        else
        {
            _extraActions = 0;
            _selectingMove++;
            _SetupSkillList(_selectingMove);
        }

        _SkillList.Index = 0;
        _UpdateStatDisplay(_selectingMove);

        States.Battle.RemoveMenu();
    }

    public static int _CheckInputTargeting(int index, int selectingMove, Range range)
    {
        // Check for confirm input
        if (InputLib.Check(Keybinds.Confirm))
        {
            return _TargetingConfirm;
        }

        int indexI = index;
        int newIndex = index;

        // Lock cursor to self for self Ranges
        if (range == Ranges.Self || range == Ranges.SelfUpDown)
        {
            if (InputLib.IsMouseLeftJustPressed() && checkClickSprites(index, newIndex) == _TargetingConfirm)
            {
                return _TargetingConfirm;
            }

            return selectingMove;
        }

        // Move selection
        if (InputLib.Check(Keybinds.Up, true))
        {
            // On player side
            if (index < PosLib.LowestOpp)
            {
                newIndex = (indexI - 1) < 0 ? PosLib.HighestAlly : index - 1;
            }
            else
            {
                newIndex = (indexI - 1) < PosLib.LowestOpp ? PosLib.HighestOpp : index - 1;
            }
        }
        else if (InputLib.Check(Keybinds.Down, true))
        {
            // On player side
            if (index < PosLib.LowestOpp)
            {
                newIndex = (indexI + 1) >= PosLib.LowestOpp ? 0 : index + 1;
            }
            else
            {
                newIndex = (indexI + 1) > PosLib.HighestOpp ? PosLib.LowestOpp : index + 1;
            }
        }
        else if (InputLib.Check(Keybinds.Left, Keybinds.Right, true))
        {
            newIndex = indexI < PosLib.LowestOpp ? index + PosLib.LowestOpp : index - PosLib.LowestOpp;
        }

        if (Settings.EnableMouse && InputLib.IsMouseLeftJustPressed())
        {
            newIndex = checkClickSprites(index, newIndex);
            if (newIndex == _TargetingConfirm)
            {
                return newIndex;
            }
        }

        // Lock cursor to valid side
        if ((range.Side == Side.Both) || (range.Side == PosLib.GetRelativeSide(selectingMove, newIndex)))
        {
            return newIndex;
        }

        return index;

        static int checkClickSprites(int index, int newIndex)
        {
            for (int i = 0; i < _Sprites.Length; i++)
            {
                if (_Sprites[i].ContainsMouse())
                {
                    newIndex = i;

                    if (index == newIndex)
                    {
                        return _TargetingConfirm;
                    }

                    break;
                }
            }

            return newIndex;
        }
    }

    #region Targeting Reticle

    private static Color _targetColor = Settings.Theme.Imp;
    private static readonly Color[] _CurColors = new Color[8];

    private static readonly Vector2 _ReticleSize = new(200);
    private const int _ReticleThickness = 20;

    private static List<int> _reticlePos = new(UnitCount);
    private static readonly Vector2[] _PrevPos = new Vector2[8];

    private const int _UninitializedReticlePos = -1;

    private static TimeSpan _animTimer = new();
    private const int _AnimDistMult = 4;
    private const int _SizeRange = 30;
    private const int _ThicknessRange = 10;

    private static void _UpdateReticle()
    {
        _targetColor = _validMainTargets[_indexTarget] ? Settings.Theme.Imp : Settings.Theme.Neg;
        _reticlePos = _selectedSkillInstance.Skill.Range.GetTargetPositions(_selectingMove, _indexTarget);
    }

    private static void _DrawReticle(GameTime gt)
    {
        _animTimer += gt.ElapsedGameTime / _AnimDistMult;
        float timer = (float) Math.Sin(_animTimer.TotalSeconds);

        Vector2 size = _ReticleSize * (float) _Reticle.Prog + new Vector2(timer * _SizeRange);

        for (int i = 0; i < _reticlePos.Count; i++)
        {
            int sPos = _reticlePos[i];

            if (sPos == PosLib.Invalid)
            {
                continue;
            }

            bool isImmune = Battle.GetUnitAtPos(sPos).GetAffinity(_selectedSkillInstance.Skill.GetElement()) >= 5;

            _CurColors[i] = Color.Lerp(_CurColors[i], isImmune ? Settings.Theme.Neg : _targetColor, RenderLib.GetInterpolationAmount(gt));

            Vector2 pos = new Vector2(sPos >= PosLib.LowestOpp ? OppSpriteX : AllySpriteX,
            GetUnitGraphicY(sPos)) + new Vector2(RenderLib.UnitSpriteSize / 2);

            if (_PrevPos[i].X != _UninitializedReticlePos)
            {
                pos = Vector2.Lerp(_PrevPos[i], pos, RenderLib.GetInterpolationAmount(gt));
            }

            _PrevPos[i] = pos;

            if (sPos == _indexTarget)
            {
                // Outer reticle
                Core.ShapeBatch.BorderRectangle(pos - size / 2, size, _CurColors[i],
                    _ReticleThickness + (timer * _ThicknessRange), rotation: timer * _AnimDistMult);
            }

            // Inner reticle
            Core.ShapeBatch.BorderRectangle(pos - (size / 4), size / 2, _CurColors[i],
                _ReticleThickness + (-timer * _ThicknessRange), rotation: -timer * _AnimDistMult);
        }
    }

    #endregion
}
