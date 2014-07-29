// ---------------------------------------------------------------------------
// <copyright file="IndexFlags.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------

// ---------------------------------------------------------------------
// <summary>
// </summary>
// ---------------------------------------------------------------------

namespace Microsoft.Isam.Esent.Isam
{
    using System;

    using Microsoft.Isam.Esent.Interop;
    using Microsoft.Isam.Esent.Interop.Vista;

    /// <summary>
    /// Index flags enumeration
    /// </summary>
    [Flags]
    public enum IndexFlags
    {
        /// <summary>
        /// The index will use the default set of flags.
        /// </summary>
        None = CreateIndexGrbit.None,

        /// <summary>
        /// All keys in the index must be unique or the insertion or update of
        /// the associated record will fail.  This uniqueness constraint also
        /// applies to truncated keys.
        /// </summary>
        Unique = CreateIndexGrbit.IndexUnique,

        /// <summary>
        /// The keys in this index represent the primary key for the record.
        /// At this time, the primary index is also the clustered index of the
        /// table.  If no primary index is defined for a table then a default
        /// primary index with a sequential key will be used.
        /// </summary>
        Primary = CreateIndexGrbit.IndexPrimary,

        /// <summary>
        /// Each key in the index must have a non-NULL value for every key
        /// column in the index definition or the insertion or update of the
        /// associated record will fail.
        /// </summary>
        DisallowNull = CreateIndexGrbit.IndexDisallowNull,

        /// <summary>
        /// The index will only contain entries for keys with at least one
        /// non-NULL key column.
        /// </summary>
        IgnoreNull = CreateIndexGrbit.IndexIgnoreNull,

        /// <summary>
        /// The index will only contain entries for keys comprised entirely of
        /// non-NULL key columns.
        /// </summary>
        IgnoreAnyNull = CreateIndexGrbit.IndexIgnoreAnyNull,

        /// <summary>
        /// The index will contain entries for keys containing any combination
        /// of NULL and non-NULL key columns.  The collation order of NULL key
        /// column values versus non-NULL key column values is determined by the
        /// SortNullsLow and SortNullsHigh IndexFlags.  This represents the
        /// default treatment of NULL key column values in an index entry.
        /// </summary>
        AllowNull = CreateIndexGrbit.None,

        /// <summary>
        /// The collation order of the index is set such that NULL key column
        /// values sort closer to the start of the index than non-NULL key
        /// column values.  This is the default NULL collation order.
        /// </summary>
        SortNullsLow = CreateIndexGrbit.None,

        /// <summary>
        /// The collation order of the index is set such that NULL key column
        /// values sort closer to the end of the index than non-NULL key column
        /// values.
        /// </summary>
        SortNullsHigh = CreateIndexGrbit.IndexSortNullsHigh,

        /// <summary>
        /// Any key in the index may be larger than the maximum size of the key
        /// supported by that index except that any key beyond the maximum size
        /// will be truncated.  This means that there can be no meaningful
        /// relative ordering between keys that are identical after truncation
        /// nor can there be any distinction between entries generated by
        /// different key column values whose keys are identical after
        /// truncation.
        /// <remarks>
        /// This bit is unique to the Isam layer. It is not actually supported
        /// by the underlying database layer. If it is not specified,
        /// <see cref="DisallowTruncation"/> is assumed. This is the opposite default
        /// of the underlying database layer
        /// <para>
        /// <see cref="AllowTruncation"/> and <see cref="DisallowTruncation"/>
        /// are mutually exclusive.
        /// </para>
        /// </remarks>
        /// </summary>
        AllowTruncation = 0x01000000,

        /// <summary>
        /// Each key in the index must be smaller than the maximum size of the
        /// key supported by that index or the insertion or update of the
        /// associated record will fail.  Note that at the present time there
        /// is some overhead when converting key column values into a key so
        /// the maximum size of the key will be reached sooner than expected
        /// based on the raw size of the key column values.  Truncated keys are
        /// prohibited by default.
        /// </summary>
        /// <remarks>
        /// This is the default behaviour for Isam, but not the default for
        /// the underlying database layer.
        /// <see cref="AllowTruncation"/> and <see cref="DisallowTruncation"/>
        /// are mutually exclusive.
        /// </remarks>
        DisallowTruncation = VistaGrbits.IndexDisallowTruncation,
    }
}
