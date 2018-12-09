﻿using System;

namespace CMSSolutions.Indexing
{
    public interface IDocumentIndex
    {
        IDocumentIndex Add(string name, string value);

        IDocumentIndex Add(string name, DateTime value);

        IDocumentIndex Add(string name, int value);

        IDocumentIndex Add(string name, bool value);

        IDocumentIndex Add(string name, double value);

        /// <summary>
        /// Stores the original value to the index.
        /// </summary>
        IDocumentIndex Store();

        /// <summary>
        /// Content is analyzed and tokenized.
        /// </summary>
        IDocumentIndex Analyze();

        /// <summary>
        /// Remove any HTML tag from the current string
        /// </summary>
        IDocumentIndex RemoveTags();

        /// <summary>
        /// Whether some property have been added to this document, or otherwise if it's empty
        /// </summary>
        bool IsDirty { get; }
    }
}