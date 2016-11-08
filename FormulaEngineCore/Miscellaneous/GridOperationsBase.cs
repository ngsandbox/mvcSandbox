using System.Diagnostics;
using System.Drawing;
using FormulaEngineCore.FormulaRererences;
using FormulaEngineCore.FormulaTypes;

namespace FormulaEngineCore.Miscellaneous
{
    /// <summary>
    ///     Encapsulates all grid related operations on a reference.  I made this a separate class so that non-grid references
    ///     don't have
    ///     to include all these methods as stubs.
    /// </summary>
    public abstract class GridOperationsBase
    {
        public abstract ReferenceOperationResultType OnColumnsInserted(int insertAt, int count);
        public abstract ReferenceOperationResultType OnColumnsRemoved(int removeAt, int count);
        public abstract ReferenceOperationResultType OnRangeMoved(SheetReference source, SheetReference dest);
        public abstract ReferenceOperationResultType OnRowsInserted(int insertAt, int count);
        public abstract ReferenceOperationResultType OnRowsRemoved(int removeAt, int count);

        protected static ReferenceOperationResultType Affected2Enum(bool affected)
        {
            if (affected)
            {
                return ReferenceOperationResultType.Affected;
            }
            return ReferenceOperationResultType.NotAffected;
        }

        /// <summary>
        ///     Handles a range move and tries to do it the same way as Excel.  This function is way too complicated but Excel has
        ///     some very
        ///     weird rules with regards to range moves and this is the only way I can think of emulating them.
        /// </summary>
        protected ReferenceOperationResultType HandleRangeMoved(SheetReference current, SheetReference source,
            SheetReference dest, SetRangeCallback callback)
        {
            Rectangle destRect = dest.Range;
            Rectangle sourceRect = source.Range;
            int rowOffset = destRect.Top - sourceRect.Top;
            int colOffset = destRect.Left - sourceRect.Left;

            Rectangle myRect = current.Range;

            bool isOnSourceSheet = ReferenceEquals(source.Sheet, current.Sheet);
            bool isOnDestSheet = ReferenceEquals(dest.Sheet, current.Sheet);

            bool sameSheet = isOnSourceSheet & isOnDestSheet;

            if (isOnSourceSheet & isOnDestSheet == false && IsEdgeMove(myRect, sourceRect))
            {
                // Move of one of our edges to another sheet
                callback(SubtractRectangle(myRect, sourceRect));
                return ReferenceOperationResultType.Affected;
            }
            if (isOnSourceSheet == false & isOnDestSheet && IsEdgeDestroyMove(myRect, sourceRect, destRect))
            {
                callback(SubtractRectangle(myRect, destRect));
                return ReferenceOperationResultType.Affected;
            }
            if (sameSheet && IsEdgeExpandMove(myRect, sourceRect, rowOffset, colOffset))
            {
                callback(GetEdgeExpandRectangle(myRect, sourceRect, destRect));
                return ReferenceOperationResultType.Affected;
            }
            if (sameSheet && IsEdgeShrinkMove(myRect, sourceRect, rowOffset, colOffset))
            {
                callback(GetEdgeShrinkRectangle(myRect, sourceRect, rowOffset, colOffset));
                return ReferenceOperationResultType.Affected;
            }
            if (sameSheet && IsEdgeDestroyMoveSameSheet(myRect, sourceRect, destRect))
            {
                callback(SubtractRectangle(myRect, destRect));
                return ReferenceOperationResultType.Affected;
            }
            if (isOnSourceSheet && sourceRect.Contains(myRect))
            {
                // Move of the whole range
                myRect.Offset(colOffset, rowOffset);
                current.SetSheetForRangeMove(dest.Sheet);
                callback(myRect);
                return ReferenceOperationResultType.Affected;
            }
            if (isOnDestSheet && destRect.Contains(myRect))
            {
                // We are overwritten by the move
                return ReferenceOperationResultType.Invalidated;
            }
            // We are affected only if the moved range intersects us
            if (current.Intersects(dest))
            {
                return ReferenceOperationResultType.Affected;
            }
            return ReferenceOperationResultType.NotAffected;
        }

// Determines if the move is of the type that pulls an edge outwards and expands the range
// Ex: range c4:e6; move c4:c6 -1col -> b4:e6
        private bool IsEdgeExpandMove(Rectangle currentRect, Rectangle sourceRect, int rowOffset, int colOffset)
        {
            Rectangle destRect = sourceRect;
            destRect.Offset(colOffset, rowOffset);

            Rectangle leftEdge = GetLeftEdge(currentRect);
            Rectangle rightEdge = GetRightEdge(currentRect);
            Rectangle topEdge = GetTopEdge(currentRect);
            Rectangle botEdge = GetBottomEdge(currentRect);

            bool isLeftRightEdgeMove = (rowOffset == 0) & (sourceRect.Contains(leftEdge) | sourceRect.Contains(rightEdge));
            bool isTopBotEdgeMove = (colOffset == 0) & (sourceRect.Contains(topEdge) | sourceRect.Contains(botEdge));

            bool isGoodRect = currentRect.IntersectsWith(sourceRect) & sourceRect.Contains(currentRect) == false &
                              currentRect.IntersectsWith(destRect) == false;

            return (isLeftRightEdgeMove | isTopBotEdgeMove) & isGoodRect;
        }

// Determines if the move is of the type that pulls an edge inwards and shrinks the range
// Ex: range c4:e6; move c4:c6 +1col -> d4:e6
        private bool IsEdgeShrinkMove(Rectangle currentRect, Rectangle sourceRect, int rowOffset, int colOffset)
        {
            Rectangle destRect = sourceRect;
            destRect.Offset(colOffset, rowOffset);

            Rectangle leftEdge = GetLeftEdge(currentRect);
            Rectangle rightEdge = GetRightEdge(currentRect);
            Rectangle topEdge = GetTopEdge(currentRect);
            Rectangle botEdge = GetBottomEdge(currentRect);

            bool isLeftRightEdgeMove = (rowOffset == 0) & (sourceRect.Contains(leftEdge) | sourceRect.Contains(rightEdge));
            bool isTopBotEdgeMove = (colOffset == 0) & (sourceRect.Contains(topEdge) | sourceRect.Contains(botEdge));

            bool isGoodRect = currentRect.IntersectsWith(destRect) & sourceRect.Contains(currentRect) == false;

            return (isLeftRightEdgeMove | isTopBotEdgeMove) & isGoodRect;
        }

// Determines if the move is of the type that has a range moved on top of the edge of our range thus
// chopping off that edge
// Ex: range c4:e6; move g4:h6 -2col -> c4:d6
        private bool IsEdgeDestroyMove(Rectangle currentRect, Rectangle sourceRect, Rectangle destRect)
        {
            Rectangle leftEdge = GetLeftEdge(currentRect);
            Rectangle rightEdge = GetRightEdge(currentRect);
            Rectangle topEdge = GetTopEdge(currentRect);
            Rectangle botEdge = GetBottomEdge(currentRect);

            bool leftEdgeGood = destRect.Contains(leftEdge) & destRect.Left < leftEdge.Left;
            bool rightEdgeGood = destRect.Contains(rightEdge) & destRect.Right > rightEdge.Right;
            bool topEdgeGood = destRect.Contains(topEdge) & destRect.Top < topEdge.Top;
            bool botEdgeGood = destRect.Contains(botEdge) & destRect.Bottom > botEdge.Bottom;

            bool xGood = (leftEdgeGood | rightEdgeGood) & sourceRect.Width > 1;
            bool yGood = (topEdgeGood | botEdgeGood) & sourceRect.Height > 1;

            bool edgeGood = xGood | yGood;
            bool rectGood = destRect.Contains(currentRect) == false;

            return edgeGood & rectGood;
        }

        private bool IsEdgeDestroyMoveSameSheet(Rectangle currentRect, Rectangle sourceRect, Rectangle destRect)
        {
            bool isMoveGood = IsEdgeDestroyMove(currentRect, sourceRect, destRect);
            bool isRectGood = currentRect.Contains(destRect) == false & sourceRect.IntersectsWith(currentRect) == false;
            return isRectGood & isMoveGood;
        }

// Gets the new rectangle after one of our edges is pulled away
        private Rectangle GetEdgeExpandRectangle(Rectangle currentRect, Rectangle sourceRect, Rectangle destRect)
        {
            Rectangle leftEdge = GetLeftEdge(currentRect);
            Rectangle rightEdge = GetRightEdge(currentRect);
            Rectangle topEdge = GetTopEdge(currentRect);
            Rectangle botEdge = GetBottomEdge(currentRect);
            Rectangle newRect = Rectangle.Empty;

            if (sourceRect.Contains(leftEdge))
            {
                newRect = Rectangle.FromLTRB(destRect.Left, currentRect.Top, currentRect.Right, currentRect.Bottom);
            }
            else if (sourceRect.Contains(rightEdge))
            {
                newRect = Rectangle.FromLTRB(currentRect.Left, currentRect.Top, destRect.Right, currentRect.Bottom);
            }
            else if (sourceRect.Contains(topEdge))
            {
                newRect = Rectangle.FromLTRB(currentRect.Left, destRect.Top, destRect.Right, currentRect.Bottom);
            }
            else if (sourceRect.Contains(botEdge))
            {
                newRect = Rectangle.FromLTRB(currentRect.Left, currentRect.Top, destRect.Right, destRect.Bottom);
            }

            Debug.Assert(newRect.IsEmpty == false);
            return newRect;
        }

// Gets the new rectangle after one of our edges is pulled in
        private Rectangle GetEdgeShrinkRectangle(Rectangle currentRect, Rectangle sourceRect, int rowOffset, int colOffset)
        {
            sourceRect.Intersect(currentRect);

            Rectangle destRect = sourceRect;
            destRect.Offset(colOffset, rowOffset);
            Rectangle newRect = SubtractRectangle(currentRect, sourceRect);
            newRect = Rectangle.Union(destRect, newRect);

            return newRect;
        }

        private bool IsEdgeMove(Rectangle currentRect, Rectangle movedRect)
        {
            Rectangle leftEdge = GetLeftEdge(currentRect);
            Rectangle rightEdge = GetRightEdge(currentRect);
            Rectangle topEdge = GetTopEdge(currentRect);
            Rectangle botEdge = GetBottomEdge(currentRect);

            bool touchesEdge = false | movedRect.Contains(leftEdge);
            touchesEdge = touchesEdge | movedRect.Contains(rightEdge);
            touchesEdge = touchesEdge | movedRect.Contains(topEdge);
            touchesEdge = touchesEdge | movedRect.Contains(botEdge);

            bool movedDoesNotContainCurrent = movedRect.Contains(currentRect) == false;

            return touchesEdge & movedDoesNotContainCurrent;
        }

        private Rectangle SubtractRectangle(Rectangle target, Rectangle toSubtract)
        {
            Rectangle leftEdge = GetLeftEdge(target);
            Rectangle rightEdge = GetRightEdge(target);
            Rectangle topEdge = GetTopEdge(target);
            Rectangle botEdge = GetBottomEdge(target);
            Rectangle newRect = Rectangle.Empty;

            if (toSubtract.Contains(leftEdge))
            {
                newRect = Rectangle.FromLTRB(toSubtract.Right, target.Top, target.Right, target.Bottom);
            }
            else if (toSubtract.Contains(rightEdge))
            {
                newRect = Rectangle.FromLTRB(target.Left, target.Top, toSubtract.Left, target.Bottom);
            }
            else if (toSubtract.Contains(topEdge))
            {
                newRect = Rectangle.FromLTRB(target.Left, toSubtract.Bottom, target.Right, target.Bottom);
            }
            else if (toSubtract.Contains(botEdge))
            {
                newRect = Rectangle.FromLTRB(target.Left, target.Top, target.Right, toSubtract.Top);
            }

            return newRect;
        }

        private Rectangle GetTopEdge(Rectangle rect)
        {
            return new Rectangle(rect.Left, rect.Top, rect.Width, 1);
        }

        private Rectangle GetBottomEdge(Rectangle rect)
        {
            return new Rectangle(rect.Left, rect.Bottom - 1, rect.Width, 1);
        }

        private Rectangle GetLeftEdge(Rectangle rect)
        {
            return new Rectangle(rect.Left, rect.Top, 1, rect.Height);
        }

        private Rectangle GetRightEdge(Rectangle rect)
        {
            return new Rectangle(rect.Right - 1, rect.Top, 1, rect.Height);
        }

        protected static Point HandleRangeInserted(int start, int finish, int insertAt, int count)
        {
            start = HandleInsert(start, insertAt, count);
            finish = HandleInsert(finish, insertAt, count);
            return new Point(start, finish);
        }

        private static int HandleInsert(int value, int insertAt, int count)
        {
            if (value >= insertAt)
            {
                return value + count;
            }
            return value;
        }

        private static int HandleRemove(int value, int removeAt, int count)
        {
            if (value > removeAt + count - 1)
            {
                // We are below the removed hole
                return value - count;
            }
            return value;
        }

        protected static Point HandleRangeRemoved(int start, int finish, int removeAt, int count)
        {
            bool containsStart = start >= removeAt & start < removeAt + count;
            bool containsFinish = finish >= removeAt & finish < removeAt + count;

            if (containsStart & containsFinish)
            {
                // The entire range we cover is removed so we are invalid
                return Point.Empty;
            }
            if (containsStart)
            {
                start = removeAt;
                finish = HandleRemove(finish, removeAt, count);
            }
            else if (containsFinish)
            {
                start = HandleRemove(start, removeAt, count);
                finish = removeAt - 1;
            }
            else
            {
                start = HandleRemove(start, removeAt, count);
                finish = HandleRemove(finish, removeAt, count);
            }

            var p = new Point(start, finish);
            return p;
        }

        protected delegate void SetRangeCallback(Rectangle newRect);
    }
}