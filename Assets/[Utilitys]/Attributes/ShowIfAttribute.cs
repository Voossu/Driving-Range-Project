using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct)]
public class ShowIfAttribute : PropertyAttribute
{
    public string SourceField;
    public bool HideInInspector;
    public bool Inverse;
    public object CompareValue;

    public ShowIfAttribute(string sourceField, object compareObject, bool inverse = false, bool hideInInspector = true)
    {
        this.SourceField = sourceField;
        this.HideInInspector = hideInInspector;
        this.Inverse = inverse;
        this.CompareValue = compareObject == null ? true : compareObject;
    }

    public ShowIfAttribute(string sourceField, bool compareValue = true, bool inverse = false, bool hideInInspector = true)
    {
        this.SourceField = sourceField;
        this.HideInInspector = hideInInspector;
        this.Inverse = inverse;
        this.CompareValue = compareValue;
    }
}