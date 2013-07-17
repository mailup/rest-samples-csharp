Imports System.Collections.Generic
Imports System.Linq
Imports System.Web

Namespace Entity
    Public Class EmailDynamicFieldDTO
        Inherits DynamicFieldDTO
        Public Property Value() As [String]
            Get
                Return m_Value
            End Get
            Set(value As [String])
                m_Value = value
            End Set
        End Property
        Private m_Value As [String]
    End Class
End Namespace
