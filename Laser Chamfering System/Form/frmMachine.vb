Public Class frmMachine
    Dim m_nPlcAliveVal(3) As Integer

    Private Sub frmMachine_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        On Error GoTo SysErr
        tmMachine.Enabled = False
        Exit Sub
SysErr:

    End Sub

    Private Sub frmMachine_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        On Error GoTo SysErr
        tmMachine.Enabled = True
        Exit Sub
SysErr:
    End Sub
    Private Sub tmMachine_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmMachine.Tick
        On Error GoTo SysErr

#If SIMUL <> 1 Then
        LaserStatus()
        ScannerStatus()
        StagePos()
        PowerMeterStatus()
        DustCollectorStatus()
        ChillerStatus()
        CuttingPos()

        If pLDLT.PC_Infomation.nPC_Alive = 0 Then

            lblStatusPLC.BackColor = Color.Lime

        ElseIf pLDLT.PC_Infomation.nPC_Alive = 1 Then

            lblStatusPLC.BackColor = Color.White

        End If

        pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_PC_4ALIGN_STATUS, False)
#End If

        Exit Sub
SysErr:

    End Sub
    Public bLaserAlram As Boolean = False

    Private Sub LaserStatus()
        On Error GoTo SysErr

        Dim Attenuator(3) As Label
        Dim Control(3) As Label
        Dim LaserStatus(3) As Label
        Dim ShutterStatus(3) As Label
        Dim Repetition(3) As Label
        Dim LaserPower(3) As Label

        Attenuator(0) = lblAtt
        Attenuator(1) = lblAtt2
        Attenuator(2) = lblAtt3
        Attenuator(3) = lblAtt4

        Control(0) = lblControl
        Control(1) = lblControl2
        Control(2) = lblControl3
        Control(3) = lblControl4

        LaserStatus(0) = lblLaserStatus
        LaserStatus(1) = lblLaserStatus2
        LaserStatus(2) = lblLaserStatus3
        LaserStatus(3) = lblLaserStatus4

        ShutterStatus(0) = lblShutter
        ShutterStatus(1) = lblShutter2
        ShutterStatus(2) = lblShutter3
        ShutterStatus(3) = lblShutter4

        Repetition(0) = lblRR
        Repetition(1) = lblRR2
        Repetition(2) = lblRR3
        Repetition(3) = lblRR4

        LaserPower(0) = lblLaserOutputPower
        LaserPower(1) = lblLaserOutputPower2
        LaserPower(2) = lblLaserOutputPower3
        LaserPower(3) = lblLaserOutputPower4

        For index As Integer = 0 To 3
            If pLaser(index).pLaserStatus.bConnect = True Then
                If pLaser(index).pLaserStatus.strPulsemode <> "" Then

                    If pLaser(index).pLaserStatus.strPulsemode <> "2" Then
                        'frmAlarm.ShowAlarmMsg(7)
                        '20190729 레이저 상태 비트 추가
                        If pLDLT.PLC_Infomation.bLaserPassMode(index) = True Then
                            Select Case index
                                Case 0
                                    If pLDLT.PC_Infomation.bLaserMode(index) = False Then
                                        pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER1_MODE, True)
                                        pLDLT.PC_Infomation.bLaserMode(index) = True
                                    End If
                                Case 1
                                    If pLDLT.PC_Infomation.bLaserMode(index) = False Then
                                        pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER2_MODE, True)
                                        pLDLT.PC_Infomation.bLaserMode(index) = True
                                    End If
                                Case 2
                                    If pLDLT.PC_Infomation.bLaserMode(index) = False Then
                                        pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER3_MODE, True)
                                        pLDLT.PC_Infomation.bLaserMode(index) = True
                                    End If
                                Case 3
                                    If pLDLT.PC_Infomation.bLaserMode(index) = False Then
                                        pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER4_MODE, True)
                                        pLDLT.PC_Infomation.bLaserMode(index) = True
                                    End If
                            End Select
                        Else
                            Select Case index
                                Case 0
                                    If pLDLT.PC_Infomation.bLaserMode(index) = True Then
                                        pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER1_MODE, False)
                                        pLDLT.PC_Infomation.bLaserMode(index) = False
                                    End If
                                Case 1
                                    If pLDLT.PC_Infomation.bLaserMode(index) = True Then
                                        pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER2_MODE, False)
                                        pLDLT.PC_Infomation.bLaserMode(index) = False
                                    End If
                                Case 2
                                    If pLDLT.PC_Infomation.bLaserMode(index) = True Then
                                        pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER3_MODE, False)
                                        pLDLT.PC_Infomation.bLaserMode(index) = False
                                    End If
                                Case 3
                                    If pLDLT.PC_Infomation.bLaserMode(index) = True Then
                                        pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER4_MODE, False)
                                        pLDLT.PC_Infomation.bLaserMode(index) = False
                                    End If
                            End Select
                        End If

                    Else
                        '20190729 레이저 상태 비트 추가
                        Select Case index
                            Case 0
                                If pLDLT.PC_Infomation.bLaserMode(index) = True Then
                                    pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER1_MODE, False)
                                    pLDLT.PC_Infomation.bLaserMode(index) = False
                                End If
                            Case 1
                                If pLDLT.PC_Infomation.bLaserMode(index) = True Then
                                    pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER2_MODE, False)
                                    pLDLT.PC_Infomation.bLaserMode(index) = False
                                End If
                            Case 2
                                If pLDLT.PC_Infomation.bLaserMode(index) = True Then
                                    pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER3_MODE, False)
                                    pLDLT.PC_Infomation.bLaserMode(index) = False
                                End If
                            Case 3
                                If pLDLT.PC_Infomation.bLaserMode(index) = True Then
                                    pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER4_MODE, False)
                                    pLDLT.PC_Infomation.bLaserMode(index) = False
                                End If
                        End Select
                    End If

                    Select Case pLaser(index).pLaserStatus.strPulsemode
                        Case "0"
                            Control(index).Text = "Internal"
                            Control(index).BackColor = Color.Red
                        Case "1"
                            Control(index).Text = "External"
                            Control(index).BackColor = Color.Red
                        Case "2"
                            Control(index).Text = "Internal & Gated"
                            Control(index).BackColor = Color.Lime
                        Case "3"
                            Control(index).Text = "External & Gated"
                            Control(index).BackColor = Color.Red
                    End Select
                End If

                If pLaser(index).pLaserStatus.strAttenuatorPower <> "" And pLaser(index).pLaserStatus.strAttenuatorPower <> "TimeOutError" Then
                    'Attenuator(index).Text = (100 - CInt(pLaser(index).pLaserStatus.strAttenuatorPower)).ToString
                    Attenuator(index).Text = pLaser(index).pLaserStatus.strRFPercentLevel
                    If pLaser(index).pLaserStatus.strRFPercentLevel > pCurSystemParam.nLaserPowerMaxLimit Or pLaser(index).pLaserStatus.strRFPercentLevel < pCurSystemParam.nLaserPowerMinLimit Then
                        AlarmSeq.bLaserPowerLimit(index) = True
                    Else
                        AlarmSeq.bLaserPowerLimit(index) = False
                    End If
                End If

                If pLaser(index).pLaserStatus.strMasterRepRate <> "" Then
                    Repetition(index).Text = pLaser(index).pLaserStatus.strMasterRepRate '.ToString
                End If

                If pLaser(index).pLaserStatus.strOutputPower <> "" Then
                    LaserPower(index).Text = pLaser(index).pLaserStatus.strOutputPower
                End If

                If pLaser(index).pLaserStatus.strState <> "" Then
                    If pLaser(index).pLaserStatus.strState = "1" Then
                        '20190729 레이저 상태 비트 추가
                        Select Case index
                            Case 0
                                If pLDLT.PC_Infomation.bLaserOn(index) = True Then
                                    pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER1_ON, False)
                                    pLDLT.PC_Infomation.bLaserOn(index) = False
                                End If
                            Case 1
                                If pLDLT.PC_Infomation.bLaserOn(index) = True Then
                                    pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER2_ON, False)
                                    pLDLT.PC_Infomation.bLaserOn(index) = False
                                End If
                            Case 2
                                If pLDLT.PC_Infomation.bLaserOn(index) = True Then
                                    pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER3_ON, False)
                                    pLDLT.PC_Infomation.bLaserOn(index) = False
                                End If
                            Case 3
                                If pLDLT.PC_Infomation.bLaserOn(index) = True Then
                                    pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER4_ON, False)
                                    pLDLT.PC_Infomation.bLaserOn(index) = False
                                End If
                        End Select
                    Else
                        '20190729 레이저 상태 비트 추가
                        If pLDLT.PLC_Infomation.bLaserPassMode(index) = True Then
                            Select Case index
                                Case 0
                                    If pLDLT.PC_Infomation.bLaserOn(index) = False Then
                                        pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER1_ON, True)
                                        pLDLT.PC_Infomation.bLaserOn(index) = False
                                    End If
                                Case 1
                                    If pLDLT.PC_Infomation.bLaserOn(index) = False Then
                                        pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER2_ON, True)
                                        pLDLT.PC_Infomation.bLaserOn(index) = False
                                    End If
                                Case 2
                                    If pLDLT.PC_Infomation.bLaserOn(index) = False Then
                                        pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER3_ON, True)
                                        pLDLT.PC_Infomation.bLaserOn(index) = False
                                    End If
                                Case 3
                                    If pLDLT.PC_Infomation.bLaserOn(index) = False Then
                                        pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER4_ON, True)
                                        pLDLT.PC_Infomation.bLaserOn(index) = False
                                    End If
                            End Select
                        Else
                            Select Case index
                                Case 0
                                    If pLDLT.PC_Infomation.bLaserOn(index) = True Then
                                        pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER1_ON, False)
                                        pLDLT.PC_Infomation.bLaserOn(index) = False
                                    End If
                                Case 1
                                    If pLDLT.PC_Infomation.bLaserOn(index) = True Then
                                        pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER2_ON, False)
                                        pLDLT.PC_Infomation.bLaserOn(index) = False
                                    End If
                                Case 2
                                    If pLDLT.PC_Infomation.bLaserOn(index) = True Then
                                        pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER3_ON, False)
                                        pLDLT.PC_Infomation.bLaserOn(index) = False
                                    End If
                                Case 3
                                    If pLDLT.PC_Infomation.bLaserOn(index) = True Then
                                        pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER4_ON, False)
                                        pLDLT.PC_Infomation.bLaserOn(index) = False
                                    End If
                            End Select
                        End If

                    End If

                    If pLaser(index).pLaserStatus.strState = "1" Then
                        LaserStatus(index).BackColor = Color.Lime
                        LaserStatus(index).Text = "On"

                    ElseIf pLaser(index).pLaserStatus.strState = "2" Then
                        LaserStatus(index).BackColor = Color.Red
                        LaserStatus(index).Text = "fault"
                        'frmAlarm.ShowAlarmMsg(10)
                    ElseIf pLaser(index).pLaserStatus.strState = "0" Then
                        LaserStatus(index).BackColor = Color.LightGray
                        LaserStatus(index).Text = "Ready"
                    End If

                End If

                If pLaser(index).pLaserStatus.strShutterState <> "" Then
                    If pLaser(index).pLaserStatus.strShutterState = "1" Then
                        ShutterStatus(index).Text = "Shutter Open"
                        ShutterStatus(index).BackColor = Color.Lime
                        '20190729 레이저 상태 비트 추가
                        Select Case index
                            Case 0
                                If pLDLT.PC_Infomation.bLaserShutter(index) = True Then
                                    pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER1_SHUTTER, False)
                                    pLDLT.PC_Infomation.bLaserShutter(index) = False
                                End If
                            Case 1
                                If pLDLT.PC_Infomation.bLaserShutter(index) = True Then
                                    pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER2_SHUTTER, False)
                                    pLDLT.PC_Infomation.bLaserShutter(index) = False
                                End If
                            Case 2
                                If pLDLT.PC_Infomation.bLaserShutter(index) = True Then
                                    pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER3_SHUTTER, False)
                                    pLDLT.PC_Infomation.bLaserShutter(index) = False
                                End If
                            Case 3
                                If pLDLT.PC_Infomation.bLaserShutter(index) = True Then
                                    pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER4_SHUTTER, False)
                                    pLDLT.PC_Infomation.bLaserShutter(index) = False
                                End If
                        End Select
                    Else
                        ShutterStatus(index).Text = "Shutter Close"
                        ShutterStatus(index).BackColor = Color.Red
                        'frmAlarm.ShowAlarmMsg(9) '하태규 기장 추가 요구
                        '20190729 레이저 상태 비트 추가
                        If pLDLT.PLC_Infomation.bLaserPassMode(index) = True Then
                            Select Case index
                                Case 0
                                    If pLDLT.PC_Infomation.bLaserShutter(index) = False Then
                                        pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER1_SHUTTER, True)
                                        pLDLT.PC_Infomation.bLaserShutter(index) = True
                                    End If
                                Case 1
                                    If pLDLT.PC_Infomation.bLaserShutter(index) = False Then
                                        pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER2_SHUTTER, True)
                                        pLDLT.PC_Infomation.bLaserShutter(index) = True
                                    End If
                                Case 2
                                    If pLDLT.PC_Infomation.bLaserShutter(index) = False Then
                                        pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER3_SHUTTER, True)
                                        pLDLT.PC_Infomation.bLaserShutter(index) = True
                                    End If
                                Case 3
                                    If pLDLT.PC_Infomation.bLaserShutter(index) = False Then
                                        pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER4_SHUTTER, True)
                                        pLDLT.PC_Infomation.bLaserShutter(index) = True
                                    End If
                            End Select
                        Else
                            Select Case index
                                Case 0
                                    If pLDLT.PC_Infomation.bLaserShutter(index) = True Then
                                        pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER1_SHUTTER, False)
                                        pLDLT.PC_Infomation.bLaserShutter(index) = False
                                    End If
                                Case 1
                                    If pLDLT.PC_Infomation.bLaserShutter(index) = True Then
                                        pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER2_SHUTTER, False)
                                        pLDLT.PC_Infomation.bLaserShutter(index) = False
                                    End If
                                Case 2
                                    If pLDLT.PC_Infomation.bLaserShutter(index) = True Then
                                        pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER3_SHUTTER, False)
                                        pLDLT.PC_Infomation.bLaserShutter(index) = False
                                    End If
                                Case 3
                                    If pLDLT.PC_Infomation.bLaserShutter(index) = True Then
                                        pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER4_SHUTTER, False)
                                        pLDLT.PC_Infomation.bLaserShutter(index) = False
                                    End If
                            End Select
                        End If

                    End If

                End If

            End If
        Next

        Exit Sub
SysErr:

    End Sub


    'GYN - ADD :: LINE 정보 받아와야 함.!!!
    '20181108 스캐너 busy 신호 A,B Line 나눠져 있어서 통합 사용
    'Dim m_nLine As Integer

    Public Sub ScannerStatus()
        On Error GoTo SysErr
        Dim lblScanWork() As Label = {lblScanWork1, lblScanWork2, lblScanWork3, lblScanWork4}
        Dim lblScanX() As Label = {lblScanX1, lblScanX2, lblScanX3, lblScanX4}
        Dim lblScanY() As Label = {lblScanY1, lblScanY2, lblScanY3, lblScanY4}
        Dim LaserBusyCheck(7) As Boolean

        '20181108 스캐너 busy 신호 A,B Line 나눠져 있어서 통합 사용
        For i = 0 To 3
            If pScanner(i).pStatus.bInit = True Then
                If pScanner(i).pStatus.bAbleWork = True And pScanner(i).pStatus.bShot = False Then
                    lblScanWork(i).Text = "Ready"
                    lblScanWork(i).BackColor = Color.Lime

                    Select Case i
                        Case 0
                            If pLDLT.PC_Infomation.bLaserBusy_1(LINE.A) = True Then
                                pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER_BUSY_A_1, False)
                                pLDLT.PC_Infomation.bLaserBusy_1(LINE.A) = False
                                modPub.MelsecLog("Trimming A Laser Busy ReSetBit : " & clsLDLT.BIT.WB_LASER_BUSY_A_1)
                                'LaserBusyCheck(0) = False
                            End If
                            If pLDLT.PC_Infomation.bLaserBusy_1(LINE.B) = True Then
                                pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER_BUSY_B_1, False)
                                pLDLT.PC_Infomation.bLaserBusy_1(LINE.B) = False
                                modPub.MelsecLog("Trimming B Laser Busy ReSetBit : " & clsLDLT.BIT.WB_LASER_BUSY_B_1)
                                'LaserBusyCheck(1) = False
                            End If

                        Case 1
                            If pLDLT.PC_Infomation.bLaserBusy_2(LINE.A) = True Then
                                pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER_BUSY_A_2, False)
                                pLDLT.PC_Infomation.bLaserBusy_2(LINE.A) = False
                                modPub.MelsecLog("Trimming A Laser Busy ReSetBit : " & clsLDLT.BIT.WB_LASER_BUSY_A_2)
                                'LaserBusyCheck(2) = False
                            End If
                            If pLDLT.PC_Infomation.bLaserBusy_2(LINE.B) = True Then
                                pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER_BUSY_B_2, False)
                                pLDLT.PC_Infomation.bLaserBusy_2(LINE.B) = False
                                modPub.MelsecLog("Trimming B Laser Busy ReSetBit : " & clsLDLT.BIT.WB_LASER_BUSY_B_2)
                                'LaserBusyCheck(3) = False
                            End If

                        Case 2
                            If pLDLT.PC_Infomation.bLaserBusy_3(LINE.A) = True Then
                                pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER_BUSY_A_3, False)
                                pLDLT.PC_Infomation.bLaserBusy_3(LINE.A) = False
                                modPub.MelsecLog("Trimming A Laser Busy ReSetBit : " & clsLDLT.BIT.WB_LASER_BUSY_A_3)
                                'LaserBusyCheck(4) = False
                            End If
                            If pLDLT.PC_Infomation.bLaserBusy_3(LINE.B) = True Then
                                pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER_BUSY_B_3, False)
                                pLDLT.PC_Infomation.bLaserBusy_3(LINE.B) = False
                                modPub.MelsecLog("Trimming B Laser Busy ReSetBit : " & clsLDLT.BIT.WB_LASER_BUSY_B_3)
                                'LaserBusyCheck(5) = False
                            End If

                        Case 3
                            If pLDLT.PC_Infomation.bLaserBusy_4(LINE.A) = True Then
                                pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER_BUSY_A_4, False)
                                pLDLT.PC_Infomation.bLaserBusy_4(LINE.A) = False
                                modPub.MelsecLog("Trimming A Laser Busy ReSetBit : " & clsLDLT.BIT.WB_LASER_BUSY_A_4)
                                'LaserBusyCheck(6) = False
                            End If
                            If pLDLT.PC_Infomation.bLaserBusy_4(LINE.B) = True Then
                                pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER_BUSY_B_4, False)
                                pLDLT.PC_Infomation.bLaserBusy_4(LINE.B) = False
                                modPub.MelsecLog("Trimming B Laser Busy ReSetBit : " & clsLDLT.BIT.WB_LASER_BUSY_B_4)
                                'LaserBusyCheck(7) = False
                            End If
                    End Select
                ElseIf pScanner(i).pStatus.bAbleWork = False Or pScanner(i).pStatus.bShot = True Then
                    lblScanWork(i).Text = "Working"
                    lblScanWork(i).BackColor = Color.Yellow
                    Select Case i
                        Case 0
                            If pLDLT.PC_Infomation.bLaserBusy_1(LINE.A) = False Then
                                pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER_BUSY_A_1, True)
                                pLDLT.PC_Infomation.bLaserBusy_1(LINE.A) = True
                                modPub.MelsecLog("Trimming A1 Laser Busy SetBit : " & clsLDLT.BIT.WB_LASER_BUSY_A_1)
                                'If LaserBusyCheck(0) = False Then
                                '    LaserBusyCheck(0) = True
                                '    TestLog("A1 Laser On")
                                'End If
                            End If
                            If pLDLT.PC_Infomation.bLaserBusy_1(LINE.B) = False Then
                                pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER_BUSY_B_1, True)
                                pLDLT.PC_Infomation.bLaserBusy_1(LINE.B) = True
                                modPub.MelsecLog("Trimming B1 Laser Busy SetBit : " & clsLDLT.BIT.WB_LASER_BUSY_B_1)
                                'If LaserBusyCheck(1) = False Then
                                '    LaserBusyCheck(1) = True
                                '    TestLog("B1 Laser On")
                                'End If
                            End If

                        Case 1
                            If pLDLT.PC_Infomation.bLaserBusy_2(LINE.A) = False Then
                                pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER_BUSY_A_2, True)
                                pLDLT.PC_Infomation.bLaserBusy_2(LINE.A) = True
                                modPub.MelsecLog("Trimming A2 Laser Busy SetBit : " & clsLDLT.BIT.WB_LASER_BUSY_A_2)
                                'If LaserBusyCheck(2) = False Then
                                '    LaserBusyCheck(2) = True
                                '    TestLog("A2 Laser On")
                                'End If
                            End If
                            If pLDLT.PC_Infomation.bLaserBusy_2(LINE.B) = False Then
                                pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER_BUSY_B_2, True)
                                pLDLT.PC_Infomation.bLaserBusy_2(LINE.B) = True
                                modPub.MelsecLog("Trimming B2 Laser Busy SetBit : " & clsLDLT.BIT.WB_LASER_BUSY_B_2)
                                'If LaserBusyCheck(3) = False Then
                                '    LaserBusyCheck(3) = True
                                '    TestLog("B2 Laser On")
                                'End If
                            End If

                        Case 2
                            If pLDLT.PC_Infomation.bLaserBusy_3(LINE.A) = False Then
                                pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER_BUSY_A_3, True)
                                pLDLT.PC_Infomation.bLaserBusy_3(LINE.A) = True
                                modPub.MelsecLog("Trimming A3 Laser Busy SetBit : " & clsLDLT.BIT.WB_LASER_BUSY_A_3)
                                'If LaserBusyCheck(4) = False Then
                                '    LaserBusyCheck(4) = True
                                '    TestLog("A3 Laser On")
                                'End If
                            End If
                            If pLDLT.PC_Infomation.bLaserBusy_3(LINE.B) = False Then
                                pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER_BUSY_B_3, True)
                                pLDLT.PC_Infomation.bLaserBusy_3(LINE.B) = True
                                modPub.MelsecLog("Trimming B3 Laser Busy SetBit : " & clsLDLT.BIT.WB_LASER_BUSY_B_3)
                                'If LaserBusyCheck(5) = False Then
                                '    LaserBusyCheck(5) = True
                                '    TestLog("B3 Laser On")
                                'End If
                            End If

                        Case 3
                            If pLDLT.PC_Infomation.bLaserBusy_4(LINE.A) = False Then
                                pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER_BUSY_A_4, True)
                                pLDLT.PC_Infomation.bLaserBusy_4(LINE.A) = True
                                modPub.MelsecLog("Trimming A4 Laser Busy SetBit : " & clsLDLT.BIT.WB_LASER_BUSY_A_4)
                                'If LaserBusyCheck(6) = False Then
                                '    LaserBusyCheck(6) = True
                                '    TestLog("A4 Laser On")
                                'End If
                            End If
                            If pLDLT.PC_Infomation.bLaserBusy_4(LINE.B) = False Then
                                pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_LASER_BUSY_B_4, True)
                                pLDLT.PC_Infomation.bLaserBusy_4(LINE.B) = True
                                modPub.MelsecLog("Trimming B4 Laser Busy SetBit : " & clsLDLT.BIT.WB_LASER_BUSY_B_4)
                                'If LaserBusyCheck(7) = False Then
                                '    LaserBusyCheck(7) = True
                                '    TestLog("B4 Laser On")
                                'End If
                            End If

                    End Select
                End If
                lblScanX(i).Text = Math.Round((pScanner(i).pStatus.dPosX / 1000), 3)
                lblScanY(i).Text = Math.Round((pScanner(i).pStatus.dPosY / 1000), 3)
            End If
        Next

        Exit Sub
SysErr:

    End Sub

    Private Sub DustCollectorStatus()

        On Error GoTo SysErr
        Static bRun As Boolean = False
        If bRun = True Then Exit Sub
        Dim Status(1) As Label
        Dim DustFreq(1) As Label

        bRun = True

        Dim lblD() As Label = {lblDust}
        For i = 0 To 0
            If pDustCollector(0).IsOpen Then
                lblD(i).Text = "Connected"
                lblD(i).BackColor = Color.Yellow

                If pDustCollector(i).Status.RunFlag = clsDustCollector.RUNFLAG.RUN Then
                    lblD(i).BackColor = Color.Green
                    lblD(i).Text = "Running"
                ElseIf CInt(pDustCollector(i).Status.RunFlag) = clsDustCollector.RUNFLAG.STOPON Then
                    lblD(i).BackColor = Color.Yellow
                    lblD(i).Text = "Stop"
                End If
            Else
                lblD(i).Text = "Not Connected"
                lblD(i).BackColor = Color.Yellow
            End If
        Next

        bRun = False
        Exit Sub
SysErr:
        bRun = False
    End Sub

    Dim tmpPLC_Word1 As Short = 0
    Dim tmpPLC_Word2 As Short = 0

    Dim tmpPC_Word1 As Short = 0
    Dim tmpPC_Word2 As Short = 0

    Private Sub StagePos()
        On Error GoTo SysErr

        If pLDLT.pbConnect = True Then
            lblPosX1.Text = (pLDLT.PLC_Infomation.dCurPosX(LINE.A)).ToString
            lblPosY1.Text = (pLDLT.PLC_Infomation.dCurPosY(LINE.A)).ToString
            lblPosVZ1.Text = (pLDLT.PLC_Infomation.dCurPosCameraZ1(LINE.A)).ToString
            lblPosLZ1.Text = (pLDLT.PLC_Infomation.dCurPosScannerZ(0)).ToString
            lblPosLZ3.Text = (pLDLT.PLC_Infomation.dCurPosScannerZ(2)).ToString

            lblPosX2.Text = (pLDLT.PLC_Infomation.dCurPosX(LINE.B)).ToString
            lblPosY2.Text = (pLDLT.PLC_Infomation.dCurPosY(LINE.B)).ToString
            lblPosVZ2.Text = (pLDLT.PLC_Infomation.dCurPosCameraZ1(LINE.B)).ToString
            lblPosLZ2.Text = (pLDLT.PLC_Infomation.dCurPosScannerZ(1)).ToString
            lblPosLZ4.Text = (pLDLT.PLC_Infomation.dCurPosScannerZ(3)).ToString
        End If

        Dim bLanguageSetting As Boolean = False
        Dim strTemp As String = ""

        strTemp = lblPosX1.Text
        If InStr(strTemp, ",") Then
            bLanguageSetting = True
        End If

        strTemp = lblPosY1.Text
        If InStr(strTemp, ",") Then
            bLanguageSetting = True
        End If

        strTemp = lblPosVZ1.Text
        If InStr(strTemp, ",") Then
            bLanguageSetting = True
        End If

        strTemp = lblPosLZ1.Text
        If InStr(strTemp, ",") Then
            bLanguageSetting = True
        End If

        strTemp = lblPosLZ3.Text
        If InStr(strTemp, ",") Then
            bLanguageSetting = True
        End If

        strTemp = lblPosX2.Text
        If InStr(strTemp, ",") Then
            bLanguageSetting = True
        End If

        strTemp = lblPosY2.Text
        If InStr(strTemp, ",") Then
            bLanguageSetting = True
        End If

        strTemp = lblPosVZ2.Text
        If InStr(strTemp, ",") Then
            bLanguageSetting = True
        End If

        strTemp = lblPosLZ2.Text
        If InStr(strTemp, ",") Then
            bLanguageSetting = True
        End If

        strTemp = lblPosLZ4.Text
        If InStr(strTemp, ",") Then
            bLanguageSetting = True
        End If

        If bLanguageSetting = True Then
            frmMSG.ShowMsg("국가 언어", "윈도우 국가 언어 설정 확인", False, 1)
        End If

        Exit Sub
SysErr:

    End Sub


    Private Sub PowerMeterStatus()
        On Error GoTo SysErr
        'Dim lblPwrMeterStatus() As Label = {lblPowerMeterStatus, lblPowerMeterStatus2, lblPowerMeterStatus3, lblPowerMeterStatus4, lblPowerMeterStatusStage}
        'Dim lblPwrMeter() As Label = {lblPowerMeter, lblPowerMeter2, lblPowerMeter3, lblPowerMeter4, lblPowerMeterStage}
        Dim tmpPower(3) As Double
        Dim tmpPowerStage(3) As Double
        Dim strPower As String

        '20190822
        '마지막 파워 데이터 저장 경로 
        Dim strFilePath As String
        strFilePath = "C:\Chamfering System\DEFAULT.ini"

        If pPowerMeter(0).PowerMeterStatus.bConnect = True Then
            If pPowerMeter(0).UsePowerMeter Then
                tmpPower(0) = Math.Round(CDbl(pPowerMeter(0).PowerMeterStatus.Power), 3)
                lblPowerMeter.Text = "OK"
                lblPowerMeter.BackColor = Color.Lime
                lblPowerMeter.Text = tmpPower(0) 'Math.Round(tmpPower(0), 3)
                'RYO(RMS & APD)
                'RMS
                pMXComponent.WriteWordDoubleByAddress(Val("&H" & 6310), tmpPower(0) * 100) 'A2
                pMXComponent.WriteWordDoubleByAddress(Val("&H" & 6340), tmpPower(0) * 100) 'B1
                'APD
                pMXComponent.WriteWordDoubleByAddress(Val("&H" & 6230), tmpPower(0) * 100) 'A2
                pMXComponent.WriteWordDoubleByAddress(Val("&H" & 6260), tmpPower(0) * 100) 'B1

                WriteIni("LASER_LAST_POWER", "LASER_TOP_1", tmpPower(0), strFilePath)

            Else
                'tmpPower = 0
                'lblPowerMeter.Text = "OK"
                lblPowerMeter.BackColor = Color.Lime
                'lblPowerMeter.Text = Math.Round(tmpPower, 3)
            End If
        Else
            lblPowerMeter.Text = "Not Connected"
            lblPowerMeter.BackColor = Color.DarkGray
            lblPowerMeter.Text = "--"
        End If

        If pPowerMeter(1).PowerMeterStatus.bConnect = True Then
            If pPowerMeter(1).UsePowerMeter Then
                tmpPower(1) = Math.Round(CDbl(pPowerMeter(1).PowerMeterStatus.Power), 3)
                lblPowerMeter2.Text = "OK"
                lblPowerMeter2.BackColor = Color.Lime
                lblPowerMeter2.Text = tmpPower(1) 'Math.Round(tmpPower(1), 3)

                'RYO
                'RMS
                pMXComponent.WriteWordDoubleByAddress(Val("&H" & 6300), tmpPower(1) * 100) 'A1
                pMXComponent.WriteWordDoubleByAddress(Val("&H" & 6350), tmpPower(1) * 100) 'B2
                'APD
                pMXComponent.WriteWordDoubleByAddress(Val("&H" & 6220), tmpPower(1) * 100) 'A1
                pMXComponent.WriteWordDoubleByAddress(Val("&H" & 6270), tmpPower(1) * 100) 'B2

                WriteIni("LASER_LAST_POWER", "LASER_TOP_2", tmpPower(1), strFilePath)
            Else
                lblPowerMeter2.BackColor = Color.Lime

            End If
        Else
            lblPowerMeter2.Text = "Not Connected"
            lblPowerMeter2.BackColor = Color.DarkGray
            lblPowerMeter2.Text = "--"
        End If

        If pPowerMeter(2).PowerMeterStatus.bConnect = True Then
            If pPowerMeter(2).UsePowerMeter Then
                tmpPower(2) = Math.Round(CDbl(pPowerMeter(2).PowerMeterStatus.Power), 3)
                lblPowerMeter3.Text = "OK"
                lblPowerMeter3.BackColor = Color.Lime
                lblPowerMeter3.Text = tmpPower(2) 'Math.Round(tmpPower(2), 3)

                'RYO
                'RMS
                pMXComponent.WriteWordDoubleByAddress(Val("&H" & 6330), tmpPower(2) * 100) 'A4
                pMXComponent.WriteWordDoubleByAddress(Val("&H" & 6360), tmpPower(2) * 100) 'B3
                'APD
                pMXComponent.WriteWordDoubleByAddress(Val("&H" & 6250), tmpPower(2) * 100) 'A4
                pMXComponent.WriteWordDoubleByAddress(Val("&H" & 6280), tmpPower(2) * 100) 'B3

                WriteIni("LASER_LAST_POWER", "LASER_TOP_3", tmpPower(2), strFilePath)
            Else
               
                lblPowerMeter3.BackColor = Color.Lime
            End If
        Else
            lblPowerMeter3.Text = "Not Connected"
            lblPowerMeter3.BackColor = Color.DarkGray
            lblPowerMeter3.Text = "--"
        End If

        If pPowerMeter(3).PowerMeterStatus.bConnect = True Then
            If pPowerMeter(3).UsePowerMeter Then
                tmpPower(3) = Math.Round(CDbl(pPowerMeter(3).PowerMeterStatus.Power), 3)
                lblPowerMeter4.Text = "OK"
                lblPowerMeter4.BackColor = Color.Lime
                lblPowerMeter4.Text = tmpPower(3) 'Math.Round(tmpPower(3), 3)

                'RYO
                'RMS
                pMXComponent.WriteWordDoubleByAddress(Val("&H" & 6320), tmpPower(3) * 100) 'A3
                pMXComponent.WriteWordDoubleByAddress(Val("&H" & 6370), tmpPower(3) * 100) 'B4
                'APD
                pMXComponent.WriteWordDoubleByAddress(Val("&H" & 6240), tmpPower(3) * 100) 'A3
                pMXComponent.WriteWordDoubleByAddress(Val("&H" & 6290), tmpPower(3) * 100) 'B4

                WriteIni("LASER_LAST_POWER", "LASER_TOP_4", tmpPower(3), strFilePath)


            Else
                lblPowerMeter4.BackColor = Color.Lime

            End If
        Else
            lblPowerMeter4.Text = "Not Connected"
            lblPowerMeter4.BackColor = Color.DarkGray
            lblPowerMeter4.Text = "--"
        End If


        If pPowerMeter(4).PowerMeterStatus.bConnect = True Then
            If pPowerMeter(4).UsePowerMeter = True Then
                If pScanner(0).pStatus.bAbleWork = False Or pScanner(0).pStatus.bShot = True Then
                    tmpPowerStage(0) = Math.Round(CDbl(pPowerMeter(4).PowerMeterStatus.Power), 3)
                    lblPowerMeterTop1.Text = tmpPowerStage(0)
                    lblPowerMeterTop1.BackColor = Color.Lime

                    'RYO
                    pMXComponent.WriteWordDoubleByAddress(Val("&H" & 6311), tmpPowerStage(0) * 100) 'A2
                    pMXComponent.WriteWordDoubleByAddress(Val("&H" & 6341), tmpPowerStage(0) * 100) 'B1
                    '원영식 선임의 요청 레이저 % 게이지가 아니라 W(와트)값 요청
                    pMXComponent.WriteWordDoubleByAddress(Val("&H" & 6312), tmpPowerStage(0) * 100) 'A2
                    pMXComponent.WriteWordDoubleByAddress(Val("&H" & 6342), tmpPowerStage(0) * 100) 'B1

                    'APD
                    pMXComponent.WriteWordDoubleByAddress(Val("&H" & 6231), tmpPowerStage(0) * 100) 'A2
                    pMXComponent.WriteWordDoubleByAddress(Val("&H" & 6261), tmpPowerStage(0) * 100) 'B1
                    '추가2018.09.12
                    'If pPowerMeterThread.pbPowerCheck = True Then
                    '    If pPowerMeterThread.pbPowerMeter_Top(0) = False Then
                    '        modPub.PowerMeterLog("PowerMeter1 Stage::" & Math.Round(CDbl(pPowerMeter(4).PowerMeterStatus.Power), 3))
                    '        pPowerMeterThread.pbPowerCheck = False
                    '    End If
                    'End If

                    WriteIni("LASER_LAST_POWER", "LASER_STATE_1", tmpPowerStage(0), strFilePath)

                End If

                If pScanner(1).pStatus.bAbleWork = False Or pScanner(1).pStatus.bShot = True Then
                    tmpPowerStage(1) = Math.Round(CDbl(pPowerMeter(4).PowerMeterStatus.Power), 3)
                    lblPowerMeterTop2.Text = tmpPowerStage(1)
                    lblPowerMeterTop2.BackColor = Color.Lime

                    'RYO
                    pMXComponent.WriteWordDoubleByAddress(Val("&H" & 6301), tmpPowerStage(1) * 100) 'A1
                    pMXComponent.WriteWordDoubleByAddress(Val("&H" & 6351), tmpPowerStage(1) * 100) 'B2
                    '추가2018.09.12
                    '원영식 선임의 요청 레이저 % 게이지가 아니라 W(와트)값 요청
                    pMXComponent.WriteWordDoubleByAddress(Val("&H" & 6302), tmpPowerStage(1) * 100) 'A1
                    pMXComponent.WriteWordDoubleByAddress(Val("&H" & 6352), tmpPowerStage(1) * 100) 'B2

                    'APD
                    pMXComponent.WriteWordDoubleByAddress(Val("&H" & 6221), tmpPowerStage(1) * 100) 'A1
                    pMXComponent.WriteWordDoubleByAddress(Val("&H" & 6271), tmpPowerStage(1) * 100) 'B2
                    '추가2018.09.12

                    'If pPowerMeterThread.pbPowerCheck = True Then
                    '    If pPowerMeterThread.pbPowerMeter_Top(1) = False Then
                    '        modPub.PowerMeterLog("PowerMeter2 Stage::" & Math.Round(CDbl(pPowerMeter(4).PowerMeterStatus.Power), 3))
                    '        pPowerMeterThread.pbPowerCheck = False
                    '    End If
                    'End If

                    WriteIni("LASER_LAST_POWER", "LASER_STATE_2", tmpPowerStage(1), strFilePath)

                End If

                If pScanner(2).pStatus.bAbleWork = False Or pScanner(2).pStatus.bShot = True Then
                    tmpPowerStage(2) = Math.Round(CDbl(pPowerMeter(4).PowerMeterStatus.Power), 3)
                    lblPowerMeterTop3.Text = tmpPowerStage(2)
                    lblPowerMeterTop3.BackColor = Color.Lime

                    'RYO
                    pMXComponent.WriteWordDoubleByAddress(Val("&H" & 6331), tmpPowerStage(2) * 100) 'A4
                    pMXComponent.WriteWordDoubleByAddress(Val("&H" & 6361), tmpPowerStage(2) * 100) 'B3
                    '추가2018.09.12
                    '원영식 선임의 요청 레이저 % 게이지가 아니라 W(와트)값 요청
                    pMXComponent.WriteWordDoubleByAddress(Val("&H" & 6332), tmpPowerStage(2) * 100) 'A4
                    pMXComponent.WriteWordDoubleByAddress(Val("&H" & 6362), tmpPowerStage(2) * 100) 'B3

                    'APD
                    pMXComponent.WriteWordDoubleByAddress(Val("&H" & 6251), tmpPowerStage(2) * 100) 'A4
                    pMXComponent.WriteWordDoubleByAddress(Val("&H" & 6281), tmpPowerStage(2) * 100) 'B3
                    '추가 2018.09.12
                    'If pPowerMeterThread.pbPowerCheck = True Then
                    '    If pPowerMeterThread.pbPowerMeter_Top(2) = False Then

                    '        modPub.PowerMeterLog("PowerMeter3 Stage::" & Math.Round(CDbl(pPowerMeter(4).PowerMeterStatus.Power), 3))
                    '        pPowerMeterThread.pbPowerCheck = False
                    '    End If
                    'End If

                    WriteIni("LASER_LAST_POWER", "LASER_STATE_3", tmpPowerStage(2), strFilePath)

                End If

                If pScanner(3).pStatus.bAbleWork = False Or pScanner(3).pStatus.bShot = True Then
                    tmpPowerStage(3) = Math.Round(CDbl(pPowerMeter(4).PowerMeterStatus.Power), 3)
                    lblPowerMeterTop4.Text = tmpPowerStage(3)
                    lblPowerMeterTop4.BackColor = Color.Lime

                    'RYO
                    pMXComponent.WriteWordDoubleByAddress(Val("&H" & 6321), tmpPowerStage(3) * 100) 'A3
                    pMXComponent.WriteWordDoubleByAddress(Val("&H" & 6371), tmpPowerStage(3) * 100) 'B4
                    '추가2018.09.12
                    '원영식 선임의 요청 레이저 % 게이지가 아니라 W(와트)값 요청
                    pMXComponent.WriteWordDoubleByAddress(Val("&H" & 6322), tmpPowerStage(3) * 100) 'A3
                    pMXComponent.WriteWordDoubleByAddress(Val("&H" & 6372), tmpPowerStage(3) * 100) 'B4

                    'APD
                    pMXComponent.WriteWordDoubleByAddress(Val("&H" & 6241), tmpPowerStage(3) * 100) 'A3
                    pMXComponent.WriteWordDoubleByAddress(Val("&H" & 6291), tmpPowerStage(3) * 100) 'B4
                    '추가2018.09.12

                    'If pPowerMeterThread.pbPowerCheck = True Then
                    '    If pPowerMeterThread.pbPowerMeter_Top(3) = False Then

                    '        modPub.PowerMeterLog("PowerMeter4 Stage::" & Math.Round(CDbl(pPowerMeter(4).PowerMeterStatus.Power), 3))
                    '        pPowerMeterThread.pbPowerCheck = False
                    '    End If
                    'End If

                    WriteIni("LASER_LAST_POWER", "LASER_STATE_4", tmpPowerStage(3), strFilePath)

                End If

            Else
                lblPowerMeterTop1.BackColor = Color.Lime
                lblPowerMeterTop2.BackColor = Color.Lime
                lblPowerMeterTop3.BackColor = Color.Lime
                lblPowerMeterTop4.BackColor = Color.Lime
            End If
        Else
            lblPowerMeterTop1.Text = "Not Connected"
            lblPowerMeterTop1.BackColor = Color.DarkGray
            lblPowerMeterTop1.Text = "--"

            lblPowerMeterTop2.Text = "Not Connected"
            lblPowerMeterTop2.BackColor = Color.DarkGray
            lblPowerMeterTop2.Text = "--"

            lblPowerMeterTop3.Text = "Not Connected"
            lblPowerMeterTop3.BackColor = Color.DarkGray
            lblPowerMeterTop3.Text = "--"

            lblPowerMeterTop4.Text = "Not Connected"
            lblPowerMeterTop4.BackColor = Color.DarkGray
            lblPowerMeterTop4.Text = "--"
        End If

        Exit Sub
SysErr:

    End Sub


    Private Sub ChillerStatus()
        On Error GoTo SysErr
        'For index As Integer = 0 To 3
        lblchiller_laser1.Text = pChiller(0).m_Status.strCurTemp
        lblchiller_laser1.BackColor = Color.DarkGray
        lblchiller_laser2.Text = pChiller(1).m_Status.strCurTemp
        lblchiller_laser2.BackColor = Color.DarkGray
        lblchiller_laser3.Text = pChiller(2).m_Status.strCurTemp
        lblchiller_laser3.BackColor = Color.DarkGray
        lblchiller_laser4.Text = pChiller(3).m_Status.strCurTemp
        lblchiller_laser4.BackColor = Color.DarkGray
        'Next

        lblChilderWF1.Text = pLaser(0).pLaserStatus.strWaterFlow
        lblChilderWF1.BackColor = Color.DarkGray
        lblChilderWF2.Text = pLaser(1).pLaserStatus.strWaterFlow
        lblChilderWF2.BackColor = Color.DarkGray
        lblChilderWF3.Text = pLaser(2).pLaserStatus.strWaterFlow
        lblChilderWF3.BackColor = Color.DarkGray
        lblChilderWF4.Text = pLaser(3).pLaserStatus.strWaterFlow
        lblChilderWF4.BackColor = Color.DarkGray

        'Chiller Alarm 발생 시 ------
        For i As Integer = 0 To 3
            If pChiller(i).bAlarm = True Then
                'frmAlarm.ShowAlarmMsg(i + 14)
            End If
        Next

        Exit Sub
SysErr:

    End Sub

    Private Sub CuttingPos()

        Dim dPosA_X As Double = 0
        Dim dPosA_Y As Double = 0
        Dim dPosB_X As Double = 0
        Dim dPosB_Y As Double = 0

        dPosA_X = (pCurRecipe.AlignResult(0, Panel.p1).dMarkDestPositionX + pCurRecipe.AlignResult(0, Panel.p2).dMarkDestPositionX _
        + pCurRecipe.AlignResult(0, Panel.p3).dMarkDestPositionX + pCurRecipe.AlignResult(0, Panel.p4).dMarkDestPositionX) / 4

        dPosA_Y = (pCurRecipe.AlignResult(0, Panel.p1).dMarkDestPositionY + pCurRecipe.AlignResult(0, Panel.p2).dMarkDestPositionY _
       + pCurRecipe.AlignResult(0, Panel.p3).dMarkDestPositionY + pCurRecipe.AlignResult(0, Panel.p4).dMarkDestPositionY) / 4

        lblCuttingPosA_X.Text = Math.Round(dPosA_X, 3)
        lblCuttingPosA_Y.Text = Math.Round(dPosA_Y, 3)

        dPosB_X = (pCurRecipe.AlignResult(1, Panel.p1).dMarkDestPositionX + pCurRecipe.AlignResult(1, Panel.p2).dMarkDestPositionX _
        + pCurRecipe.AlignResult(1, Panel.p3).dMarkDestPositionX + pCurRecipe.AlignResult(1, Panel.p4).dMarkDestPositionX) / 4

        dPosB_Y = (pCurRecipe.AlignResult(1, Panel.p1).dMarkDestPositionY + pCurRecipe.AlignResult(1, Panel.p2).dMarkDestPositionY _
       + pCurRecipe.AlignResult(1, Panel.p3).dMarkDestPositionY + pCurRecipe.AlignResult(1, Panel.p4).dMarkDestPositionY) / 4

        lblCuttingPosB_X.Text = Math.Round(dPosB_X, 3)
        lblCuttingPosB_Y.Text = Math.Round(dPosB_Y, 3)

    End Sub

    Public Sub Form_Label2()
        With Me

            '.lalPassward.Text = My.Resources.setLan.ResourceManager.GetObject("Password")

        End With
    End Sub

    Dim nTest As Double = 0
    Dim nTestTemp As Double = 0
    Dim nMarkNo As Integer = 0
    Dim dPositionX(1, 3, 10) As Double
    Dim dPositionY(1, 3, 10) As Double

    Private Sub BtnTestA_Click(sender As System.Object, e As System.EventArgs) Handles BtnTestA.Click
        Dim StrLog As String = ""
        Dim dX As Double = 0.0
        Dim dY As Double = 0.0

        dX = 100.1 + nTestTemp
        dY = 102.1 + nTestTemp

        If nTest > 10 Then

            For i As Integer = 0 To 10
                dPositionX(0, 0, i) = 0
                dPositionY(0, 0, i) = 0
            Next

            nTestTemp = nTestTemp + 0.1
            nTest = 0
        Else
            dPositionX(0, 0, nTest) = dX
            dPositionY(0, 0, nTest) = dY

            nTestTemp = nTestTemp - 1
            nTest = nTest + 1
        End If

        If nMarkNo = 0 Then
            StrLog = "MARK1:: " & dX.ToString & ", " & dY.ToString
            nMarkNo = 1
        ElseIf nMarkNo = 1 Then
            dX = dX + 100
            dY = dY + 100
            StrLog = "MARK2:: " & dX.ToString & ", " & dY.ToString
            nMarkNo = 0
        End If

        AlignDataLog(0, 0, StrLog)

    End Sub

    Private Sub BtnTestB_Click(sender As System.Object, e As System.EventArgs) Handles BtnTestB.Click
        Dim StrLog As String = ""
        Dim dX As Double = 0.0
        Dim dY As Double = 0.0

        dX = 100.1 + nTestTemp
        dY = 102.1 + nTestTemp

        If nTest > 10 Then

            For i As Integer = 0 To 10
                dPositionX(0, 0, i) = 0
                dPositionY(0, 0, i) = 0
            Next

            nTestTemp = nTestTemp + 0.1
            nTest = 0
        Else
            dPositionX(0, 0, nTest) = dX
            dPositionY(0, 0, nTest) = dY

            nTestTemp = nTestTemp - 1
            nTest = nTest + 1
        End If

        If nMarkNo = 0 Then
            StrLog = "MARK1:: " & dX.ToString & ", " & dY.ToString
            nMarkNo = 1
        ElseIf nMarkNo = 1 Then
            dX = dX + 100
            dY = dY + 100
            StrLog = "MARK2:: " & dX.ToString & ", " & dY.ToString
            nMarkNo = 0
        End If

        'StrLog = dX.ToString & ", " & dY.ToString
        AlignDataLog(1, 0, StrLog)

    End Sub



    Private Sub BtnCalc_Click(sender As System.Object, e As System.EventArgs) Handles BtnAlignDataView.Click

        frmAlignDataView.Show()

    End Sub



    Public Sub LanChange(ByVal StrCulture As String)

        Me.Text = StrCulture

        With Me

            .GroupBox1.Text = My.Resources.setLan.ResourceManager.GetObject("SYSTEMSTATUS")

            .Label17.Text = My.Resources.setLan.ResourceManager.GetObject("LaserStatus")
            .Label22.Text = My.Resources.setLan.ResourceManager.GetObject("ScannerStatus")
            .Label7.Text = My.Resources.setLan.ResourceManager.GetObject("Scanner1")
            .Label18.Text = My.Resources.setLan.ResourceManager.GetObject("Scanner2")

            .Label29.Text = My.Resources.setLan.ResourceManager.GetObject("Scanner3")
            .Label32.Text = My.Resources.setLan.ResourceManager.GetObject("Scanner4")
            .Label1.Text = My.Resources.setLan.ResourceManager.GetObject("PowerMeter")
            .Label6.Text = My.Resources.setLan.ResourceManager.GetObject("TopPower")

            .Label41.Text = My.Resources.setLan.ResourceManager.GetObject("StagePower")
            .Label8.Text = My.Resources.setLan.ResourceManager.GetObject("CurrentPower")
            .Label11.Text = My.Resources.setLan.ResourceManager.GetObject("CurrentPower")
            .Label9.Text = My.Resources.setLan.ResourceManager.GetObject("CurrentPower")

            .Label40.Text = My.Resources.setLan.ResourceManager.GetObject("CurrentPower")

            .Label20.Text = My.Resources.setLan.ResourceManager.GetObject("MotionStatus")
            .lblStatusPLC.Text = My.Resources.setLan.ResourceManager.GetObject("Alive")
            .Label13.Text = My.Resources.setLan.ResourceManager.GetObject("LineAStageX")
            .Label19.Text = My.Resources.setLan.ResourceManager.GetObject("LineAStageY")

            .Label30.Text = My.Resources.setLan.ResourceManager.GetObject("LineAVisionZ")

            .Label14.Text = My.Resources.setLan.ResourceManager.GetObject("LineALaserZ1")
            .Label24.Text = My.Resources.setLan.ResourceManager.GetObject("LineALaserZ2")
            .Label3.Text = My.Resources.setLan.ResourceManager.GetObject("LineBStageX")
            .Label66.Text = My.Resources.setLan.ResourceManager.GetObject("LineBStageY")

            .Label76.Text = My.Resources.setLan.ResourceManager.GetObject("LineBVisionZ")
            .Label27.Text = My.Resources.setLan.ResourceManager.GetObject("LineBLaserZ1")

            .Label12.Text = My.Resources.setLan.ResourceManager.GetObject("LineBLaserZ2")
            .Label42.Text = My.Resources.setLan.ResourceManager.GetObject("DustCollector")

            .Label4.Text = My.Resources.setLan.ResourceManager.GetObject("Chiller")

            .Label51.Text = My.Resources.setLan.ResourceManager.GetObject("Chiller1")
            .Label54.Text = My.Resources.setLan.ResourceManager.GetObject("Chiller2")
            .Label55.Text = My.Resources.setLan.ResourceManager.GetObject("Chiller3")
            .Label56.Text = My.Resources.setLan.ResourceManager.GetObject("Chiller4")

            .lblDust.Text = My.Resources.setLan.ResourceManager.GetObject("NOTREADY")
            .lblCuttingPosA.Text = My.Resources.setLan.ResourceManager.GetObject("ACuttingPosmm")

            .lblCuttingPosB.Text = My.Resources.setLan.ResourceManager.GetObject("BCuttingPosmm")

        End With

    End Sub

    Private Sub BtnTest_Click(sender As System.Object, e As System.EventArgs) Handles BtnTest.Click

      

    End Sub
   
End Class