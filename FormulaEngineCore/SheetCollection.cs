using System;
using System.Collections;
using System.Linq;
using System.Runtime.Serialization;
using FormulaEngineCore.FormulaTypes;

namespace FormulaEngineCore
{
    /// <summary>
    ///     Manages all worksheets within the formula engine
    /// </summary>
    /// <remarks>
    ///     This class is responsible for managing the worksheets that are referenced by formulas.  It provides various
    ///     collection-oriented methods
    ///     for managing sheets.  Any sheet that you wish to be able to be used in a formula must be registered with this
    ///     class.
    /// </remarks>
    [Serializable]
    public class SheetCollection : ISerializable
    {
        private const int VERSION = 1;
        private readonly FormulaEngine _owner;
        private readonly IList _sheets;
        private ISheet _activeSheet;

        internal SheetCollection(FormulaEngine owner)
        {
            _owner = owner;
            _sheets = new ArrayList();
        }

        private SheetCollection(SerializationInfo info, StreamingContext context)
        {
            _owner = (FormulaEngine) info.GetValue("Engine", typeof (FormulaEngine));
            _activeSheet = (ISheet) info.GetValue("ActiveSheet", typeof (ISheet));
            _sheets = (IList) info.GetValue("Sheets", typeof (IList));
        }

        /// <summary>
        ///     Gets or sets the active sheet of the formula engine
        /// </summary>
        /// <value>The active sheet</value>
        /// <remarks>
        ///     The active sheet is the sheet used when none is specified.  When creating references via the
        ///     <see cref="T:ciloci.FormulaEngine.ReferenceFactory" />,
        ///     this is the sheet that will be used unless one is specified.  The same applies when creating formulas
        ///     that have sheet references that do not explicitly specify a sheet.  This property can be modified at any time.
        ///     <note>The value of this property can only be set to a sheet already contained in the collection</note>
        /// </remarks>
        /// <exception cref="System.ArgumentException">The property was assigned a sheet that is not in the collection</exception>
        /// <exception cref="System.ArgumentNullException">The property was assigned a null reference</exception>
        public ISheet ActiveSheet
        {
            get { return _activeSheet; }
            set
            {
                FormulaEngine.ValidateNonNull(value, "value");
                if (Contains(value) == false)
                {
                    throw new ArgumentException("The active sheet must exist in the sheet collection");
                }
                _activeSheet = value;
            }
        }

        /// <summary>
        ///     Gets a sheet at an index in the collection
        /// </summary>
        /// <param name="index">The index of the sheet to retrieve</param>
        /// <value>The sheet at the specified index</value>
        /// <remarks>This property is a simple indexer into the collection</remarks>
        /// <exception cref="System.ArgumentOutOfRangeException">index is negative or greater than the index of the last sheet</exception>
        public ISheet this[int index]
        {
            get
            {
                if (index < 0 | index >= _sheets.Count)
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                return (ISheet) _sheets[index];
            }
        }

        /// <summary>
        ///     Gets the number of sheets in the collection
        /// </summary>
        /// <value>The number of sheets in the collection</value>
        /// <remarks>Use this property when you need to get a count of the number of sheets registered with the formula engine</remarks>
        public int Count
        {
            get { return _sheets.Count; }
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Version", VERSION);
            info.AddValue("Engine", _owner);
            info.AddValue("Sheets", _sheets);
            info.AddValue("ActiveSheet", _activeSheet);
        }

        /// <summary>
        ///     Adds a sheet to the formula engine
        /// </summary>
        /// <param name="sheet">The sheet you wish to add</param>
        /// <remarks>
        ///     This method registers a sheet with the formula engine.  If sheet is the first sheet in the collection, then it is
        ///     set as the <see cref="P:ciloci.FormulaEngine.SheetCollection.ActiveSheet" />.
        /// </remarks>
        /// <exception cref="System.ArgumentNullException">
        ///     sheet is null
        ///     <para>The sheet's name is null</para>
        /// </exception>
        /// <exception cref="System.ArgumentException">
        ///     The sheet already exists in the collection
        ///     <para>A sheet with the same name already exists in the collection</para>
        /// </exception>
        public void Add(ISheet sheet)
        {
            InsertSheet(sheet, Count);
        }

        private void InsertSheet(ISheet sheet, int index)
        {
            FormulaEngine.ValidateNonNull(sheet, "sheet");

            if (Contains(sheet))
            {
                throw new ArgumentException("The sheet is already contained in the collection");
            }

            FormulaEngine.ValidateNonNull(sheet.Name, "sheet.Name");

            if (GetSheetByName(sheet.Name) != null)
            {
                throw new ArgumentException("A sheet with that name already exists");
            }

            _sheets.Insert(index, sheet);

            if (Count == 1)
            {
                _activeSheet = sheet;
            }
        }

        /// <summary>
        ///     Removes a sheet from the collection
        /// </summary>
        /// <param name="sheet">The sheet to remove</param>
        /// <remarks>
        ///     This method unregisters a sheet from the formula engine.  All references on the removed sheet will become invalid
        ///     and all formulas using those references will be recalculated.
        /// </remarks>
        /// <exception cref="System.ArgumentException">The sheet is not contained in the collection</exception>
        public void Remove(ISheet sheet)
        {
            FormulaEngine.ValidateNonNull(sheet, "sheet");
            if (Contains(sheet) == false)
            {
                throw new ArgumentException("Sheet not in list");
            }
            _sheets.Remove(sheet);
            _owner.OnSheetRemoved(sheet);
            if (Count == 0)
            {
                _activeSheet = null;
            }
        }

        /// <summary>
        ///     Inserts a sheet into the collection
        /// </summary>
        /// <param name="index">The index to insert the sheet at</param>
        /// <param name="sheet">The sheet to insert</param>
        /// <remarks>This method lets you insert a sheet at a particular index</remarks>
        /// <exception cref="System.ArgumentOutOfRangeException">index is negative or greater than the sheet count</exception>
        /// <exception cref="System.ArgumentNullException">
        ///     sheet is null
        ///     <para>The sheet's name is null</para>
        /// </exception>
        /// <exception cref="System.ArgumentException">
        ///     The sheet already exists in the collection
        ///     <para>A sheet with the same name already exists in the collection</para>
        /// </exception>
        public void Insert(int index, ISheet sheet)
        {
            if (index < 0 | index > Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            InsertSheet(sheet, index);
        }

        /// <summary>
        ///     Determines if the collection contains a sheet
        /// </summary>
        /// <param name="sheet">The sheet to test</param>
        /// <returns>True if the collection contains the sheet; False otherwise</returns>
        /// <remarks>This method lets you test whether a particular sheet is contained in the collection</remarks>
        public bool Contains(ISheet sheet)
        {
            return _sheets.Contains(sheet);
        }

        /// <summary>
        ///     Gets a sheet by name
        /// </summary>
        /// <param name="name">The name of the desired sheet</param>
        /// <returns>An instance of a sheet with the same name; a null reference if no sheet with the name is found</returns>
        /// <remarks>This method lets you get an instance of a sheet by specifying its name</remarks>
        public ISheet GetSheetByName(string name)
        {
            FormulaEngine.ValidateNonNull(name, "name");
            return _sheets.Cast<ISheet>().FirstOrDefault(sheet => name.Equals(sheet.Name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        ///     Gets the index of a sheet in the collection
        /// </summary>
        /// <param name="sheet">The sheet whose index you wish to get</param>
        /// <returns>The index of the sheet; -1 if sheet is not contained in the collection</returns>
        /// <remarks>A simple method that allows you to get the index of a sheet</remarks>
        public int IndexOf(ISheet sheet)
        {
            for (int i = 0; i <= _sheets.Count - 1; i++)
            {
                if (ReferenceEquals(sheet, _sheets[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        internal void Clear()
        {
            _sheets.Clear();
            _activeSheet = null;
        }
    }
}