// <copyright file="GlobalSuppressions.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics.CodeAnalysis;

// TODO: Re-evaluate this rule on the next version of the analyzer
[assembly: SuppressMessage(
    "CodeQuality",
    "IDE0079:Remove unnecessary suppression",
    Justification = "There seems to be some analyzer error with this rule.",
    Scope = "module")]