Imports System.Collections.Generic
Imports System.Linq
Imports System.Web

Namespace Entity
    Public Class DynamicFieldDTO
        Public Property Id() As Int32
            Get
                Return m_Id
            End Get
            Set(value As Int32)
                m_Id = value
            End Set
        End Property
        Private m_Id As Int32
        Public Property Description() As [String]
            Get
                Return m_Description
            End Get
            Set(value As [String])
                m_Description = value
            End Set
        End Property
        Private m_Description As [String]
    End Class
End Namespace
