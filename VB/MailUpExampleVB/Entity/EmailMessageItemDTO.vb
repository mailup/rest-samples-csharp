Imports System.Collections.Generic
Imports System.Linq
Imports System.Web

Namespace Entity
    Public Class EmailMessageItemDTO
        Public Property idList() As Int32
            Get
                Return m_idList
            End Get
            Set(value As Int32)
                m_idList = value
            End Set
        End Property
        Private m_idList As Int32
        Public Property idNL() As Int32
            Get
                Return m_idNL
            End Get
            Set(value As Int32)
                m_idNL = value
            End Set
        End Property
        Private m_idNL As Int32
        Public Property Subject() As [String]
            Get
                Return m_Subject
            End Get
            Set(value As [String])
                m_Subject = value
            End Set
        End Property
        Private m_Subject As [String]
        Public Property Notes() As [String]
            Get
                Return m_Notes
            End Get
            Set(value As [String])
                m_Notes = value
            End Set
        End Property
        Private m_Notes As [String]
        Public Property Content() As [String]
            Get
                Return m_Content
            End Get
            Set(value As [String])
                m_Content = value
            End Set
        End Property
        Private m_Content As [String]
        Public Property Fields() As List(Of EmailDynamicFieldDTO)
            Get
                Return m_Fields
            End Get
            Set(value As List(Of EmailDynamicFieldDTO))
                m_Fields = value
            End Set
        End Property
        Private m_Fields As List(Of EmailDynamicFieldDTO)
        Public Property Tags() As List(Of EmailTagDTO)
            Get
                Return m_Tags
            End Get
            Set(value As List(Of EmailTagDTO))
                m_Tags = value
            End Set
        End Property
        Private m_Tags As List(Of EmailTagDTO)
        Public Property Embed() As [Boolean]
            Get
                Return m_Embed
            End Get
            Set(value As [Boolean])
                m_Embed = value
            End Set
        End Property
        Private m_Embed As [Boolean]
        Public Property IsConfirmation() As [Boolean]
            Get
                Return m_IsConfirmation
            End Get
            Set(value As [Boolean])
                m_IsConfirmation = value
            End Set
        End Property
        Private m_IsConfirmation As [Boolean]
        Public TrackingInfo As New EmailTrackingInfoDTO()
        Public Property Head() As [String]
            Get
                Return m_Head
            End Get
            Set(value As [String])
                m_Head = value
            End Set
        End Property
        Private m_Head As [String]
        Private _body As [String]
        Public Property Body() As [String]
            Get
                Return _body
            End Get
            Set(value As [String])
                _body = value
            End Set
        End Property
        Public Sub New()
            _body = "<body>"
        End Sub
    End Class
End Namespace
