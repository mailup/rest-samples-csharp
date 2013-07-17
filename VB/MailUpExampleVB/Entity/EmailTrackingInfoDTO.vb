Imports System.Collections.Generic
Imports System.Linq
Imports System.Web

Namespace Entity
    Public Class EmailTrackingInfoDTO
        Public Property Enabled() As [Boolean]
            Get
                Return m_Enabled
            End Get
            Set(value As [Boolean])
                m_Enabled = value
            End Set
        End Property
        Private m_Enabled As [Boolean]
        Public Property Protocols() As List(Of [String])
            Get
                Return m_Protocols
            End Get
            Set(value As List(Of [String]))
                m_Protocols = value
            End Set
        End Property
        Private m_Protocols As List(Of [String])
        Public Property CustomParams() As [String]
            Get
                Return m_CustomParams
            End Get
            Set(value As [String])
                m_CustomParams = value
            End Set
        End Property
        Private m_CustomParams As [String]

        Public Function ProtocolsToString() As [String]
            Dim ret As [String] = ""
            For p As Int32 = 0 To Protocols.Count - 1
                Dim curProtocol As [String] = Protocols(p)

                ret += "" & curProtocol.Replace(":", "") & ":"
                If p < (Protocols.Count - 1) Then
                    ret += "|"
                End If
            Next

            Return ret
        End Function
        Public Function CustomParamsToString() As [String]
            Dim ret As [String] = ""
            If Not [String].IsNullOrEmpty(CustomParams) Then
                If CustomParams.StartsWith("?") Then
                    ret = CustomParams.Substring(1)
                Else
                    ret = CustomParams
                End If
            End If
            Return ret
        End Function
    End Class
End Namespace
