Imports System.Collections.Generic
Imports System.Linq
Imports System.Web

Namespace Entity
    Public Class EmailTagDTO
        Public Property Id() As Int32
            Get
                Return m_Id
            End Get
            Set(value As Int32)
                m_Id = value
            End Set
        End Property
        Private m_Id As Int32
        Public Property Name() As [String]
            Get
                Return m_Name
            End Get
            Set(value As [String])
                m_Name = value
            End Set
        End Property
        Private m_Name As [String]
    End Class
End Namespace
