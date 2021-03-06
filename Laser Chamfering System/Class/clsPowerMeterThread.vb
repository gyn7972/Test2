Imports System.Threading
Public Class clsPowerMeterThread
    Dim tmpMsg As String = ""
    Dim tmpSplit() As String = {}

    Public UsePowerMeter As Boolean

    '20160727 JCM Edit Scanner1 파워 메터 레이저 온 확인 비트
    Public pnPowerMeterSleepCnt(3) As Integer
    Public pbPowerMeterLaserOff(3) As Boolean
    Public pbPowerMeterOn(3) As Boolean

    Public pbPowerMeter_Top(3) As Boolean
    Public pnPowerMeterIndex(3) As Integer

    Public nPowerMeterCheckIndex As Integer = 0
    Public nPowerMeterCheck As Integer = 0
    Public bPowerMeterCheckChange As Boolean = False
    Public pbPowerCheck As Boolean = False
    Dim m_TimeCheck As New clsTimer
    Dim tmpStrAddress As String = ""
#Region "Thread"
    Private ThreadPowerMeter As Thread
    Private bThreadRunningPowerMeter As Boolean
    Private nKillThread As Integer
    Private qPowerMeterCmd As New Queue
    Dim bRead As Boolean
    Dim strPower As String

    ReadOnly Property ThreadRunningLaser() As Boolean
        Get
            Return bThreadRunningPowerMeter
        End Get
    End Property

    Function StartPowerMeter() As Boolean
        On Error GoTo SysErr

        For i As Integer = 0 To 3
            pnPowerMeterSleepCnt(i) = 0
            pnPowerMeterIndex(i) = 0
            pbPowerMeter_Top(i) = False
            pbPowerMeterOn(i) = False
            pbPowerMeterLaserOff(i) = False
        Next

        ThreadPowerMeter = New Thread(AddressOf PowerMeterStatusThreadSub)
        nKillThread = 0
        ThreadPowerMeter.SetApartmentState(ApartmentState.MTA)
        ThreadPowerMeter.Priority = ThreadPriority.Lowest
        ThreadPowerMeter.Start()
        StartPowerMeter = True

        UsePowerMeter = False

        Exit Function
SysErr:
        StartPowerMeter = False
        'ErrorMsg = ErrorMsg & "Start Power Meter Thread Error" & ","
    End Function

    Function EndPowerMeter() As Boolean
        On Error GoTo SysErr

        UsePowerMeter = False

        Interlocked.Exchange(nKillThread, 1)
        If bThreadRunningPowerMeter = True Then
            ThreadPowerMeter.Join(1000)
        End If

        ThreadPowerMeter = Nothing
        Return True
        Exit Function
SysErr:
        EndPowerMeter = False
        ' ErrorMsg = ErrorMsg & "End Power Meter Thread Error" & ","
    End Function

    Dim nSeqIndex As Integer = 0

    Sub PowerMeterStatusThreadSub()
        On Error GoTo SysErr
        Dim strCmd As String
        While nKillThread = 0

            '여기에 PowerMeter 넣으면... 끝?
            Select Case nSeqIndex
                Case 0
                    pPowerMeter(0).PowerMeterStatusThreadSub()  '상부

                    'PowerMeter 상/하부 측정 Seq 
                    'PowerMeter_Laser1()

                    nSeqIndex = 1
                Case 1
                    pPowerMeter(1).PowerMeterStatusThreadSub()

                    'PowerMeter 상/하부 측정 Seq 
                    'PowerMeter_Laser2()

                    nSeqIndex = 2
                Case 2
                    pPowerMeter(2).PowerMeterStatusThreadSub()

                    'PowerMeter 상/하부 측정 Seq 
                    'PowerMeter_Laser3()

                    nSeqIndex = 3
                Case 3
                    pPowerMeter(3).PowerMeterStatusThreadSub()

                    'PowerMeter 상/하부 측정 Seq 
                    'PowerMeter_Laser4()

                    nSeqIndex = 4
                Case 4  '하부 파워메타
                    pPowerMeter(4).PowerMeterStatusThreadSub()  '하부\
                    nSeqIndex = 0
            End Select

            'PowerMeter_Laser1()
            'PowerMeter_Laser2()
            'PowerMeter_Laser3()
            'PowerMeter_Laser4()

            Thread.Sleep(200)

        End While

        bThreadRunningPowerMeter = False
        Exit Sub
SysErr:
        bThreadRunningPowerMeter = False
        'ErrorMsg = ErrorMsg & "Power Meter Status Thread Error" & ","
    End Sub

    Sub Close()

        If Not (ThreadPowerMeter Is Nothing) Then
            ThreadPowerMeter.Join(10000)
        End If

        Call Finalize()

    End Sub
#End Region

    Public Sub PowerMeter_Laser1()

        On Error GoTo SysErr
        If pbPowerMeterLaserOff(0) = True Then
            pnPowerMeterSleepCnt(0) = 0
            pbPowerMeterLaserOff(0) = False
            pnPowerMeterIndex(0) = 10
        End If

        Select Case pnPowerMeterIndex(0)
            Case 0

            Case 1
                If pLDLT.PLC_Infomation.bPLC_ManualMode(LINE.A) = True And pLDLT.PLC_Infomation.bPLC_ManualMode(LINE.B) = True Then
                    If pbPowerMeter_Top(0) = True Then  '상하부 구분 설정 변수.
                        pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_MOVE_POWER_MEASURE_TOP_REQUEST_LASER1, True)
                        pScanner(0).RTC6ABSMove(0, 0)
                    ElseIf pbPowerMeter_Top(0) = False Then
                        pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_MOVE_POWER_MEASURE_REQUEST_LASER1, True)
                        pScanner(0).RTC6ABSMove(0, 0)
                    End If
                    pnPowerMeterIndex(0) = 2
                End If

            Case 2
                If pScanner(0).pStatus.bAbleWork = True Then    'Scanner 작동 안하는 상태임.
                    pnPowerMeterIndex(0) = 3
                End If

            Case 3
                If pLDLT.PLC_Infomation.bLaserPowerMoveComplete_Top_1 = True And pLDLT.PLC_Infomation.bLaserPowerMoveComplete_1 = False Then

                    If pLDLT.PC_Infomation.bLaserPowerMoveRequest_Top_1 = True Then
                        pScanner(0).RTC6LaserShotOn()

                        'pPowerMeter(0).UsePowerMeter = True
                        'pPowerMeter(4).UsePowerMeter = True
                        If pbPowerMeter_Top(0) = True Then
                            pPowerMeter(0).UsePowerMeter = True
                            pPowerMeter(4).UsePowerMeter = False
                        ElseIf pbPowerMeter_Top(0) = False Then
                            pPowerMeter(0).UsePowerMeter = False
                            pPowerMeter(4).UsePowerMeter = True
                        End If

                        pbPowerMeterOn(0) = True
                        pnPowerMeterIndex(0) = 4
                    End If
                End If

                If pLDLT.PLC_Infomation.bLaserPowerMoveComplete_1 = True And pLDLT.PLC_Infomation.bLaserPowerMoveComplete_Top_1 = False Then

                    If pLDLT.PC_Infomation.bLaserPowerMoveRequest_1 = True Then
                        pScanner(0).RTC6LaserShotOn()

                        'pPowerMeter(0).UsePowerMeter = True
                        'pPowerMeter(4).UsePowerMeter = True
                        If pbPowerMeter_Top(0) = True Then
                            pPowerMeter(0).UsePowerMeter = True
                            pPowerMeter(4).UsePowerMeter = False
                        ElseIf pbPowerMeter_Top(0) = False Then
                            pPowerMeter(0).UsePowerMeter = False
                            pPowerMeter(4).UsePowerMeter = True
                        End If

                        pbPowerMeterOn(0) = True
                        pnPowerMeterIndex(0) = 4
                    End If
                End If

            Case 4
                If m_TimeCheck.IsTimeOut(pCurSystemParam.nPowerMeterTime) = True Then
                    'If m_TimeCheck.IsTimeOut(10000) = True Then
                    pPowerMeter(0).UsePowerMeter = False
                    pPowerMeter(4).UsePowerMeter = False

                    pnPowerMeterIndex(0) = 10
                End If

            Case 10

                Threading.Thread.Sleep(500)

                pScanner(0).RTC6LaserShotOff()
                pbPowerCheck = True
                If pPowerMeterThread.pbPowerCheck = True Then
                    If pPowerMeterThread.pbPowerMeter_Top(0) = False Then
                        modPub.PowerMeterLog("PowerMeter1 Stage::" & Math.Round(CDbl(pPowerMeter(4).PowerMeterStatus.Power), 3))

                        pPowerMeterThread.pbPowerCheck = False
                    ElseIf pPowerMeterThread.pbPowerMeter_Top(0) = True Then
                        strPower = Math.Round(CDbl(pPowerMeter(0).PowerMeterStatus.Power), 3)

                        modPub.PowerMeterLog("PowerMeter1 Top::" & Math.Round(CDbl(pPowerMeter(0).PowerMeterStatus.Power), 3))

                        pPowerMeterThread.pbPowerCheck = False
                    End If
                End If
                pnPowerMeterIndex(0) = 11

            Case 11
                If pbPowerMeter_Top(0) = True Then
                    pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_MOVE_POWER_MEASURE_TOP_REQUEST_LASER1, False)
                ElseIf pbPowerMeter_Top(0) = False Then
                    pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_MOVE_POWER_MEASURE_REQUEST_LASER1, False)
                End If
                pnPowerMeterIndex(0) = 12

            Case 12
                pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_POWER_MEASURE_COMPLETE, True)
                pnPowerMeterIndex(0) = 13

            Case 13
                If pLDLT.PLC_Infomation.bLaserPowerMoveComplete_Top_1 = False And pLDLT.PLC_Infomation.bLaserPowerMoveComplete_1 = False Then

                    pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_POWER_MEASURE_COMPLETE, False)
                    pnPowerMeterIndex(0) = 14
                End If

            Case 14

                pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_MOVE_POWER_MEASURE_TOP_REQUEST_LASER1, False)
                pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_MOVE_POWER_MEASURE_REQUEST_LASER1, False)

                pPowerMeter(0).UsePowerMeter = False
                pPowerMeter(4).UsePowerMeter = False

                pbPowerMeterOn(0) = False
                pnPowerMeterIndex(0) = 0
                'End If
        End Select

        Exit Sub
SysErr:

    End Sub

    Public Sub PowerMeter_Laser2()
        On Error GoTo SysErr
        If pbPowerMeterLaserOff(1) = True Then
            pnPowerMeterSleepCnt(1) = 0
            pbPowerMeterLaserOff(1) = False
            pnPowerMeterIndex(1) = 10
        End If

        Select Case pnPowerMeterIndex(1)
            Case 0

            Case 1
                If pLDLT.PLC_Infomation.bPLC_ManualMode(LINE.A) = True And pLDLT.PLC_Infomation.bPLC_ManualMode(LINE.B) = True Then
                    If pbPowerMeter_Top(1) = True Then
                        pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_MOVE_POWER_MEASURE_TOP_REQUEST_LASER2, True)
                        pScanner(1).RTC6ABSMove(0, 0)
                    ElseIf pbPowerMeter_Top(1) = False Then
                        pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_MOVE_POWER_MEASURE_REQUEST_LASER2, True)
                        pScanner(1).RTC6ABSMove(0, 0)
                    End If
                    pnPowerMeterIndex(1) = 2
                End If

            Case 2
                If pScanner(1).pStatus.bAbleWork = True Then
                    pnPowerMeterIndex(1) = 3
                End If

            Case 3
                If pLDLT.PLC_Infomation.bLaserPowerMoveComplete_Top_2 = True And pLDLT.PLC_Infomation.bLaserPowerMoveComplete_2 = False Then

                    If pLDLT.PC_Infomation.bLaserPowerMoveRequest_Top_2 = True Then
                        pScanner(1).RTC6LaserShotOn()

                        'pPowerMeter(1).UsePowerMeter = True
                        'pPowerMeter(4).UsePowerMeter = True
                        If pbPowerMeter_Top(1) = True Then
                            pPowerMeter(1).UsePowerMeter = True
                            pPowerMeter(4).UsePowerMeter = False
                        ElseIf pbPowerMeter_Top(1) = False Then
                            pPowerMeter(1).UsePowerMeter = False
                            pPowerMeter(4).UsePowerMeter = True
                        End If

                        pbPowerMeterOn(1) = True
                        pnPowerMeterIndex(1) = 4
                    End If
                End If

                If pLDLT.PLC_Infomation.bLaserPowerMoveComplete_2 = True And pLDLT.PLC_Infomation.bLaserPowerMoveComplete_Top_2 = False Then

                    If pLDLT.PC_Infomation.bLaserPowerMoveRequest_2 = True Then
                        pScanner(1).RTC6LaserShotOn()

                        'pPowerMeter(1).UsePowerMeter = True
                        'pPowerMeter(4).UsePowerMeter = True
                        If pbPowerMeter_Top(1) = True Then
                            pPowerMeter(1).UsePowerMeter = True
                            pPowerMeter(4).UsePowerMeter = False
                        ElseIf pbPowerMeter_Top(1) = False Then
                            pPowerMeter(1).UsePowerMeter = False
                            pPowerMeter(4).UsePowerMeter = True
                        End If

                        pbPowerMeterOn(1) = True
                        pnPowerMeterIndex(1) = 4
                    End If
                End If

            Case 4
                If m_TimeCheck.IsTimeOut(pCurSystemParam.nPowerMeterTime) = True Then
                    'If m_TimeCheck.IsTimeOut(10000) = True Then
                    pPowerMeter(1).UsePowerMeter = False
                    pPowerMeter(4).UsePowerMeter = False

                    pnPowerMeterIndex(1) = 10
                End If

            Case 10

                Threading.Thread.Sleep(500)

                pScanner(1).RTC6LaserShotOff()
                pbPowerCheck = True
                If pPowerMeterThread.pbPowerCheck = True Then
                    If pPowerMeterThread.pbPowerMeter_Top(1) = False Then
                        modPub.PowerMeterLog("PowerMeter2 Stage::" & Math.Round(CDbl(pPowerMeter(4).PowerMeterStatus.Power), 3))

                        pPowerMeterThread.pbPowerCheck = False
                    ElseIf pPowerMeterThread.pbPowerMeter_Top(1) = True Then
                        strPower = Math.Round(CDbl(pPowerMeter(1).PowerMeterStatus.Power), 3)

                        modPub.PowerMeterLog("PowerMeter2 Top::" & Math.Round(CDbl(pPowerMeter(1).PowerMeterStatus.Power), 3))

                        pPowerMeterThread.pbPowerCheck = False
                    End If
                End If
                pnPowerMeterIndex(1) = 11

            Case 11
                If pbPowerMeter_Top(1) = True Then
                    pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_MOVE_POWER_MEASURE_TOP_REQUEST_LASER2, False)
                ElseIf pbPowerMeter_Top(1) = False Then
                    pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_MOVE_POWER_MEASURE_REQUEST_LASER2, False)
                End If
                pnPowerMeterIndex(1) = 12

            Case 12
                pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_POWER_MEASURE_COMPLETE, True)
                pnPowerMeterIndex(1) = 13

            Case 13
                If pLDLT.PLC_Infomation.bLaserPowerMoveComplete_Top_2 = False And pLDLT.PLC_Infomation.bLaserPowerMoveComplete_2 = False Then

                    pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_POWER_MEASURE_COMPLETE, False)
                    pnPowerMeterIndex(1) = 14
                End If

            Case 14

                pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_MOVE_POWER_MEASURE_TOP_REQUEST_LASER2, False)
                pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_MOVE_POWER_MEASURE_REQUEST_LASER2, False)

                pPowerMeter(1).UsePowerMeter = False
                pPowerMeter(4).UsePowerMeter = False

                pbPowerMeterOn(1) = False
                pnPowerMeterIndex(1) = 0
                'End If
        End Select

        Exit Sub
SysErr:

    End Sub

    Public Sub PowerMeter_Laser3()
        On Error GoTo SysErr
        If pbPowerMeterLaserOff(2) = True Then
            pnPowerMeterSleepCnt(2) = 0
            pbPowerMeterLaserOff(2) = False
            pnPowerMeterIndex(2) = 10
        End If

        Select Case pnPowerMeterIndex(2)
            Case 0

            Case 1
                If pLDLT.PLC_Infomation.bPLC_ManualMode(LINE.A) = True And pLDLT.PLC_Infomation.bPLC_ManualMode(LINE.B) = True Then
                    If pbPowerMeter_Top(2) = True Then
                        pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_MOVE_POWER_MEASURE_TOP_REQUEST_LASER3, True)
                        pScanner(2).RTC6ABSMove(0, 0)
                    ElseIf pbPowerMeter_Top(2) = False Then
                        pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_MOVE_POWER_MEASURE_REQUEST_LASER3, True)
                        pScanner(2).RTC6ABSMove(0, 0)
                    End If
                    pnPowerMeterIndex(2) = 2
                End If

            Case 2
                If pScanner(2).pStatus.bAbleWork = True Then
                    pnPowerMeterIndex(2) = 3
                End If

            Case 3
                If pLDLT.PLC_Infomation.bLaserPowerMoveComplete_Top_3 = True And pLDLT.PLC_Infomation.bLaserPowerMoveComplete_3 = False Then

                    If pLDLT.PC_Infomation.bLaserPowerMoveRequest_Top_3 = True Then
                        pScanner(2).RTC6LaserShotOn()

                        'pPowerMeter(2).UsePowerMeter = True
                        'pPowerMeter(4).UsePowerMeter = True
                        If pbPowerMeter_Top(2) = True Then
                            pPowerMeter(2).UsePowerMeter = True
                            pPowerMeter(4).UsePowerMeter = False
                        ElseIf pbPowerMeter_Top(2) = False Then
                            pPowerMeter(2).UsePowerMeter = False
                            pPowerMeter(4).UsePowerMeter = True
                        End If

                        pbPowerMeterOn(2) = True
                        pnPowerMeterIndex(2) = 4
                    End If
                End If

                If pLDLT.PLC_Infomation.bLaserPowerMoveComplete_3 = True And pLDLT.PLC_Infomation.bLaserPowerMoveComplete_Top_3 = False Then

                    If pLDLT.PC_Infomation.bLaserPowerMoveRequest_3 = True Then
                        pScanner(2).RTC6LaserShotOn()

                        'pPowerMeter(2).UsePowerMeter = True
                        'pPowerMeter(4).UsePowerMeter = True
                        If pbPowerMeter_Top(2) = True Then
                            pPowerMeter(2).UsePowerMeter = True
                            pPowerMeter(4).UsePowerMeter = False
                        ElseIf pbPowerMeter_Top(2) = False Then
                            pPowerMeter(2).UsePowerMeter = False
                            pPowerMeter(4).UsePowerMeter = True
                        End If

                        pbPowerMeterOn(2) = True
                        pnPowerMeterIndex(2) = 4
                    End If
                End If

            Case 4
                If m_TimeCheck.IsTimeOut(pCurSystemParam.nPowerMeterTime) = True Then
                    'If m_TimeCheck.IsTimeOut(10000) = True Then
                    pPowerMeter(2).UsePowerMeter = False
                    pPowerMeter(4).UsePowerMeter = False

                    pnPowerMeterIndex(2) = 10
                End If

            Case 10

                Threading.Thread.Sleep(500)

                pScanner(2).RTC6LaserShotOff()
                pbPowerCheck = True
                If pPowerMeterThread.pbPowerCheck = True Then
                    If pPowerMeterThread.pbPowerMeter_Top(2) = False Then
                        modPub.PowerMeterLog("PowerMeter3 Stage::" & Math.Round(CDbl(pPowerMeter(4).PowerMeterStatus.Power), 3))

                        pPowerMeterThread.pbPowerCheck = False
                    ElseIf pPowerMeterThread.pbPowerMeter_Top(2) = True Then
                        strPower = Math.Round(CDbl(pPowerMeter(2).PowerMeterStatus.Power), 3)

                        pPowerMeterThread.pbPowerCheck = False
                    End If
                End If
                pnPowerMeterIndex(2) = 11

            Case 11
                If pbPowerMeter_Top(2) = True Then
                    pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_MOVE_POWER_MEASURE_TOP_REQUEST_LASER3, False)
                ElseIf pbPowerMeter_Top(2) = False Then
                    pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_MOVE_POWER_MEASURE_REQUEST_LASER3, False)
                End If
                pnPowerMeterIndex(2) = 12

            Case 12
                pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_POWER_MEASURE_COMPLETE, True)
                pnPowerMeterIndex(2) = 13

            Case 13
                If pLDLT.PLC_Infomation.bLaserPowerMoveComplete_Top_3 = False And pLDLT.PLC_Infomation.bLaserPowerMoveComplete_3 = False Then

                    pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_POWER_MEASURE_COMPLETE, False)
                    pnPowerMeterIndex(2) = 14
                End If

            Case 14

                pbPowerMeterOn(2) = False

                pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_MOVE_POWER_MEASURE_TOP_REQUEST_LASER3, False)
                pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_MOVE_POWER_MEASURE_REQUEST_LASER3, False)

                pPowerMeter(2).UsePowerMeter = False
                pPowerMeter(4).UsePowerMeter = False
                pnPowerMeterIndex(2) = 0
                'End If
        End Select

        Exit Sub
SysErr:

    End Sub

    Public Sub PowerMeter_Laser4()
        On Error GoTo SysErr
        If pbPowerMeterLaserOff(3) = True Then
            pnPowerMeterSleepCnt(3) = 0
            pbPowerMeterLaserOff(3) = False
            pnPowerMeterIndex(3) = 10
        End If

        Select Case pnPowerMeterIndex(3)
            Case 0

            Case 1
                If pLDLT.PLC_Infomation.bPLC_ManualMode(LINE.A) = True And pLDLT.PLC_Infomation.bPLC_ManualMode(LINE.B) = True Then
                    If pbPowerMeter_Top(3) = True Then
                        pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_MOVE_POWER_MEASURE_TOP_REQUEST_LASER4, True)
                        pScanner(3).RTC6ABSMove(0, 0)
                    ElseIf pbPowerMeter_Top(3) = False Then
                        pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_MOVE_POWER_MEASURE_REQUEST_LASER4, True)
                        pScanner(3).RTC6ABSMove(0, 0)
                    End If
                    pnPowerMeterIndex(3) = 2
                End If

            Case 2
                If pScanner(3).pStatus.bAbleWork = True Then
                    pnPowerMeterIndex(3) = 3
                End If

            Case 3
                If pLDLT.PLC_Infomation.bLaserPowerMoveComplete_Top_4 = True And pLDLT.PLC_Infomation.bLaserPowerMoveComplete_4 = False Then

                    If pLDLT.PC_Infomation.bLaserPowerMoveRequest_Top_4 = True Then
                        pScanner(3).RTC6LaserShotOn()

                        'pPowerMeter(3).UsePowerMeter = True
                        'pPowerMeter(4).UsePowerMeter = True
                        If pbPowerMeter_Top(3) = True Then
                            pPowerMeter(3).UsePowerMeter = True
                            pPowerMeter(4).UsePowerMeter = False
                        ElseIf pbPowerMeter_Top(3) = False Then
                            pPowerMeter(3).UsePowerMeter = False
                            pPowerMeter(4).UsePowerMeter = True
                        End If

                        pbPowerMeterOn(3) = True
                        pnPowerMeterIndex(3) = 4

                    End If
                End If

                If pLDLT.PLC_Infomation.bLaserPowerMoveComplete_4 = True And pLDLT.PLC_Infomation.bLaserPowerMoveComplete_Top_4 = False Then

                    If pLDLT.PC_Infomation.bLaserPowerMoveRequest_4 = True Then
                        pScanner(3).RTC6LaserShotOn()

                        'pPowerMeter(3).UsePowerMeter = True
                        'pPowerMeter(4).UsePowerMeter = True
                        If pbPowerMeter_Top(3) = True Then
                            pPowerMeter(3).UsePowerMeter = True
                            pPowerMeter(4).UsePowerMeter = False
                        ElseIf pbPowerMeter_Top(3) = False Then
                            pPowerMeter(3).UsePowerMeter = False
                            pPowerMeter(4).UsePowerMeter = True
                        End If

                        pbPowerMeterOn(3) = True
                        pnPowerMeterIndex(3) = 4
                    End If
                End If

            Case 4
                If m_TimeCheck.IsTimeOut(pCurSystemParam.nPowerMeterTime) = True Then
                    pnPowerMeterIndex(3) = 10

                    pPowerMeter(3).UsePowerMeter = False
                    pPowerMeter(4).UsePowerMeter = False


                End If

            Case 10

                Threading.Thread.Sleep(500)

                pScanner(3).RTC6LaserShotOff()
                pbPowerCheck = True
                If pPowerMeterThread.pbPowerCheck = True Then
                    If pPowerMeterThread.pbPowerMeter_Top(3) = False Then
                        modPub.PowerMeterLog("PowerMeter4 Stage::" & Math.Round(CDbl(pPowerMeter(4).PowerMeterStatus.Power), 3))

                        pPowerMeterThread.pbPowerCheck = False
                    ElseIf pPowerMeterThread.pbPowerMeter_Top(3) = True Then
                        strPower = Math.Round(CDbl(pPowerMeter(3).PowerMeterStatus.Power), 3)

                        modPub.PowerMeterLog("PowerMeter4 Top::" & Math.Round(CDbl(pPowerMeter(3).PowerMeterStatus.Power), 3))

                        pPowerMeterThread.pbPowerCheck = False
                    End If
                End If
                pnPowerMeterIndex(3) = 11

            Case 11
                If pbPowerMeter_Top(3) = True Then
                    pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_MOVE_POWER_MEASURE_TOP_REQUEST_LASER4, False)
                ElseIf pbPowerMeter_Top(3) = False Then
                    pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_MOVE_POWER_MEASURE_REQUEST_LASER4, False)
                End If
                pnPowerMeterIndex(3) = 12

            Case 12
                pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_POWER_MEASURE_COMPLETE, True)
                pnPowerMeterIndex(3) = 13

            Case 13
                If pLDLT.PLC_Infomation.bLaserPowerMoveComplete_Top_4 = False And pLDLT.PLC_Infomation.bLaserPowerMoveComplete_4 = False Then

                    pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_POWER_MEASURE_COMPLETE, False)
                    pnPowerMeterIndex(3) = 14
                End If

            Case 14

                pbPowerMeterOn(3) = False

                pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_MOVE_POWER_MEASURE_TOP_REQUEST_LASER4, False)
                pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_MOVE_POWER_MEASURE_REQUEST_LASER4, False)

                pPowerMeter(3).UsePowerMeter = False
                pPowerMeter(4).UsePowerMeter = False

                pnPowerMeterIndex(3) = 0
                'End If
        End Select

        Exit Sub
SysErr:

    End Sub

    Public Sub Laser1EnergyCalibration()
        On Error GoTo SysErr
        If pbPowerMeterLaserOff(0) = True Then
            pnPowerMeterSleepCnt(0) = 0
            pbPowerMeterLaserOff(0) = False
            pnPowerMeterIndex(0) = 10
        End If

        Select Case pnPowerMeterIndex(0)
            Case 0

            Case 1
                If pLDLT.PLC_Infomation.bPLC_ManualMode(LINE.A) = True And pLDLT.PLC_Infomation.bPLC_ManualMode(LINE.B) = True Then
                    If pbPowerMeter_Top(0) = True Then  '상하부 구분 설정 변수.
                        pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_MOVE_POWER_MEASURE_TOP_REQUEST_LASER1, True)
                        pScanner(0).RTC6ABSMove(0, 0)
                    ElseIf pbPowerMeter_Top(0) = False Then
                        pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_MOVE_POWER_MEASURE_REQUEST_LASER1, True)
                        pScanner(0).RTC6ABSMove(0, 0)
                    End If
                    pnPowerMeterIndex(0) = 2
                End If

            Case 2
                If pScanner(0).pStatus.bAbleWork = True Then    'Scanner 작동 안하는 상태임.
                    pnPowerMeterIndex(0) = 3
                End If

            Case 3
                If pLDLT.PLC_Infomation.bLaserPowerMoveComplete_Top_1 = True And pLDLT.PLC_Infomation.bLaserPowerMoveComplete_1 = False Then

                    pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_MOVE_POWER_MEASURE_TOP_REQUEST_LASER1, False)
                    pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_MOVE_POWER_MEASURE_REQUEST_LASER1, False)

                    If pLDLT.PC_Infomation.bLaserPowerMoveRequest_Top_1 = True Then
                        pScanner(0).RTC6LaserShotOn()

                        'pPowerMeter(0).UsePowerMeter = True
                        'pPowerMeter(4).UsePowerMeter = True

                        If pbPowerMeter_Top(0) = True Then
                            pPowerMeter(0).UsePowerMeter = True
                        ElseIf pbPowerMeter_Top(0) = False Then
                            pPowerMeter(4).UsePowerMeter = True
                        End If

                        pbPowerMeterOn(0) = True
                        pnPowerMeterIndex(0) = 4
                    End If
                End If

                If pLDLT.PLC_Infomation.bLaserPowerMoveComplete_1 = True And pLDLT.PLC_Infomation.bLaserPowerMoveComplete_Top_1 = False Then

                    pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_MOVE_POWER_MEASURE_TOP_REQUEST_LASER1, False)
                    pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_MOVE_POWER_MEASURE_REQUEST_LASER1, False)

                    If pLDLT.PC_Infomation.bLaserPowerMoveRequest_1 = True Then
                        pScanner(0).RTC6LaserShotOn()

                        pPowerMeter(0).UsePowerMeter = True
                        pPowerMeter(4).UsePowerMeter = True

                        If pbPowerMeter_Top(0) = True Then
                            pPowerMeter(0).UsePowerMeter = True
                        ElseIf pbPowerMeter_Top(0) = False Then
                            pPowerMeter(4).UsePowerMeter = True
                        End If


                        pbPowerMeterOn(0) = True
                        pnPowerMeterIndex(0) = 4
                    End If
                End If

            Case 4

                pbPowerCheck = True
                If pPowerMeterThread.pbPowerCheck = True Then
                    If pPowerMeterThread.pbPowerMeter_Top(0) = False Then
                        If m_TimeCheck.IsTimeOut(1000) = True Then
                            modPub.PowerMeterLog("PowerMeter1 Stage::" & Math.Round(CDbl(pPowerMeter(4).PowerMeterStatus.Power), 3))
                        End If
                        If m_TimeCheck.IsTimeOut(1000) = True Then
                            modPub.PowerMeterLog("PowerMeter1 Stage::" & Math.Round(CDbl(pPowerMeter(4).PowerMeterStatus.Power), 3))
                        End If
                        If m_TimeCheck.IsTimeOut(1000) = True Then
                            modPub.PowerMeterLog("PowerMeter1 Stage::" & Math.Round(CDbl(pPowerMeter(4).PowerMeterStatus.Power), 3))
                        End If
                        If m_TimeCheck.IsTimeOut(1000) = True Then
                            modPub.PowerMeterLog("PowerMeter1 Stage::" & Math.Round(CDbl(pPowerMeter(4).PowerMeterStatus.Power), 3))
                        End If
                        If m_TimeCheck.IsTimeOut(1000) = True Then
                            modPub.PowerMeterLog("PowerMeter1 Stage::" & Math.Round(CDbl(pPowerMeter(4).PowerMeterStatus.Power), 3))
                        End If

                        pPowerMeterThread.pbPowerCheck = False
                    End If
                End If

                pnPowerMeterIndex(0) = 10


            Case 10

                pScanner(0).RTC6LaserShotOff()

                pnPowerMeterIndex(0) = 11

            Case 11
                If pbPowerMeter_Top(0) = True Then
                    pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_MOVE_POWER_MEASURE_TOP_REQUEST_LASER1, False)
                ElseIf pbPowerMeter_Top(0) = False Then
                    pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_MOVE_POWER_MEASURE_REQUEST_LASER1, False)
                End If
                pnPowerMeterIndex(0) = 12

            Case 12
                pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_POWER_MEASURE_COMPLETE, True)
                pnPowerMeterIndex(0) = 13

            Case 13
                If pLDLT.PLC_Infomation.bLaserPowerMoveComplete_Top_1 = False And pLDLT.PLC_Infomation.bLaserPowerMoveComplete_1 = False Then

                    pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_POWER_MEASURE_COMPLETE, False)
                    pnPowerMeterIndex(0) = 14
                End If

            Case 14

                pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_MOVE_POWER_MEASURE_TOP_REQUEST_LASER1, False)
                pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_MOVE_POWER_MEASURE_REQUEST_LASER1, False)

                pPowerMeter(0).UsePowerMeter = False
                pPowerMeter(4).UsePowerMeter = False

                pbPowerMeterOn(0) = False
                pnPowerMeterIndex(0) = 0
                'End If
        End Select

        Exit Sub
SysErr:

    End Sub



    Private m_nStartTick As Integer
    Private m_bStarted As Boolean = False
    Private Function IsWaitTime(ByVal nIntervalMil As Integer) As Boolean       '이걸로 1분 기다리기 할까?
        If m_bStarted = False Then
            m_bStarted = True
            m_nStartTick = My.Computer.Clock.TickCount
        Else
            If (My.Computer.Clock.TickCount - m_nStartTick) > nIntervalMil Then
                m_nStartTick = 0
                m_bStarted = False
                Return True
            End If
        End If

        Return False
    End Function

    Public Sub ResetPowerMeter()

        'pScanner(0).RTC6LaserShotOff()
        'pScanner(1).RTC6LaserShotOff()
        'pScanner(2).RTC6LaserShotOff()
        'pScanner(3).RTC6LaserShotOff()

        'For i As Integer = 0 To 3
        '    pnPowerMeterSleepCnt(i) = 0
        '    pnPowerMeterIndex(i) = 0
        '    pbPowerMeterLaserOff(i) = False
        '    pbPowerMeterOn(i) = False
        '    pbPowerMeter_Top(i) = False
        'Next
        m_TimeCheck.Reset()
        pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_MOVE_POWER_MEASURE_TOP_REQUEST_LASER1, False)
        pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_MOVE_POWER_MEASURE_TOP_REQUEST_LASER2, False)
        pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_MOVE_POWER_MEASURE_TOP_REQUEST_LASER3, False)
        pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_MOVE_POWER_MEASURE_TOP_REQUEST_LASER4, False)

        pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_MOVE_POWER_MEASURE_REQUEST_LASER1, False)
        pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_MOVE_POWER_MEASURE_REQUEST_LASER2, False)
        pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_MOVE_POWER_MEASURE_REQUEST_LASER3, False)
        pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_MOVE_POWER_MEASURE_REQUEST_LASER4, False)

        pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_POWER_MEASURE_COMPLETE, False)


    End Sub

    Public Sub CheckStagePowerMeter()
        On Error GoTo SysErr
        If bPowerMeterCheckChange = True Then
            nPowerMeterCheckIndex = 1
            bPowerMeterCheckChange = False
        End If

        Select Case nPowerMeterCheckIndex
            Case 0

            Case 1
                Select Case nPowerMeterCheck
                    Case 1
                        nPowerMeterCheck = 10
                    Case 2
                        nPowerMeterCheck = 20
                    Case 3
                        nPowerMeterCheck = 30
                    Case 4
                        nPowerMeterCheck = 40
                End Select

            Case 10
                If pbPowerMeterOn(0) = True And pbPowerMeter_Top(0) = True Then 'Check Power Meter 1 Use
                    pbPowerMeterLaserOff(0) = True
                End If
                If pbPowerMeterOn(1) = True And pbPowerMeter_Top(1) = False Then 'Check Power Meter 2 Use
                    pbPowerMeterLaserOff(1) = True
                End If
                If pbPowerMeterOn(2) = True And pbPowerMeter_Top(2) = False Then 'Check Power Meter 3 Use
                    pbPowerMeterLaserOff(2) = True
                End If
                If pbPowerMeterOn(3) = True And pbPowerMeter_Top(3) = False Then 'Check Power Meter 4 Use
                    pbPowerMeterLaserOff(3) = True
                End If
                nPowerMeterCheck = 11

            Case 11
                If pbPowerMeterOn(1) = True And pbPowerMeter_Top(1) = True Then 'Check Power Meter 2 Off
                    nPowerMeterCheck = 12
                ElseIf pbPowerMeterOn(1) = False Then
                    nPowerMeterCheck = 12
                End If

            Case 12
                If pbPowerMeterOn(2) = True And pbPowerMeter_Top(2) = True Then 'Check Power Meter 3 Off
                    nPowerMeterCheck = 13
                ElseIf pbPowerMeterOn(2) = False Then
                    nPowerMeterCheck = 13
                End If

            Case 13
                If pbPowerMeterOn(3) = True And pbPowerMeter_Top(3) = True Then 'Check Power Meter 4 Off
                    nPowerMeterCheck = 14
                ElseIf pbPowerMeterOn(3) = False Then
                    nPowerMeterCheck = 14
                End If

            Case 14
                If pbPowerMeterOn(0) = False Then 'Check Power Meter 1 Use
                    nPowerMeterCheck = 15
                ElseIf pbPowerMeterOn(0) = True And pbPowerMeter_Top(0) = False Then
                    nPowerMeterCheck = 15
                End If

            Case 15
                pbPowerMeter_Top(0) = False
                pnPowerMeterIndex(0) = 1
                nPowerMeterCheck = 0

            Case 20
                If pbPowerMeterOn(0) = True And pbPowerMeter_Top(0) = False Then 'Check Power Meter 1 Use
                    pbPowerMeterLaserOff(0) = True
                End If
                If pbPowerMeterOn(1) = True And pbPowerMeter_Top(1) = True Then 'Check Power Meter 2 Use
                    pbPowerMeterLaserOff(1) = True
                End If
                If pbPowerMeterOn(2) = True And pbPowerMeter_Top(2) = False Then 'Check Power Meter 3 Use
                    pbPowerMeterLaserOff(2) = True
                End If
                If pbPowerMeterOn(3) = True And pbPowerMeter_Top(3) = False Then 'Check Power Meter 4 Use
                    pbPowerMeterLaserOff(3) = True
                End If
                nPowerMeterCheck = 21

            Case 21
                If pbPowerMeterOn(0) = True And pbPowerMeter_Top(0) = True Then 'Check Power Meter 1 Off
                    nPowerMeterCheck = 22
                ElseIf pbPowerMeterOn(0) = False Then
                    nPowerMeterCheck = 22
                End If

            Case 22
                If pbPowerMeterOn(2) = True And pbPowerMeter_Top(2) = True Then 'Check Power Meter 3 Off
                    nPowerMeterCheck = 23
                ElseIf pbPowerMeterOn(2) = False Then
                    nPowerMeterCheck = 23
                End If

            Case 23
                If pbPowerMeterOn(3) = True And pbPowerMeter_Top(3) = True Then 'Check Power Meter 4 Off
                    nPowerMeterCheck = 24
                ElseIf pbPowerMeterOn(3) = False Then
                    nPowerMeterCheck = 24
                End If

            Case 24
                If pbPowerMeterOn(1) = False Then 'Check Power Meter 2 Use
                    nPowerMeterCheck = 25
                ElseIf pbPowerMeterOn(1) = True And pbPowerMeter_Top(1) = False Then
                    nPowerMeterCheck = 0
                End If

            Case 25
                pbPowerMeter_Top(1) = False
                pnPowerMeterIndex(1) = 1
                nPowerMeterCheck = 0

            Case 30
                If pbPowerMeterOn(0) = True And pbPowerMeter_Top(0) = False Then 'Check Power Meter 1 Use
                    pbPowerMeterLaserOff(0) = True
                End If
                If pbPowerMeterOn(1) = True And pbPowerMeter_Top(1) = False Then 'Check Power Meter 2 Use
                    pbPowerMeterLaserOff(1) = True
                End If
                If pbPowerMeterOn(2) = True And pbPowerMeter_Top(2) = True Then 'Check Power Meter 3 Use
                    pbPowerMeterLaserOff(2) = True
                End If
                If pbPowerMeterOn(3) = True And pbPowerMeter_Top(3) = False Then 'Check Power Meter 4 Use
                    pbPowerMeterLaserOff(3) = True
                End If
                nPowerMeterCheck = 31

            Case 31
                If pbPowerMeterOn(0) = True And pbPowerMeter_Top(0) = True Then 'Check Power Meter 1 Off
                    nPowerMeterCheck = 32
                ElseIf pbPowerMeterOn(0) = False Then
                    nPowerMeterCheck = 32
                End If

            Case 32
                If pbPowerMeterOn(1) = True And pbPowerMeter_Top(1) = True Then 'Check Power Meter 2 Off
                    nPowerMeterCheck = 33
                ElseIf pbPowerMeterOn(1) = False Then
                    nPowerMeterCheck = 33
                End If

            Case 33
                If pbPowerMeterOn(3) = True And pbPowerMeter_Top(3) = True Then 'Check Power Meter 4 Off
                    nPowerMeterCheck = 34
                ElseIf pbPowerMeterOn(3) = False Then
                    nPowerMeterCheck = 34
                End If

            Case 34
                If pbPowerMeterOn(2) = False Then 'Check Power Meter 3 Use
                    nPowerMeterCheck = 35
                ElseIf pbPowerMeterOn(2) = True And pbPowerMeter_Top(2) = False Then
                    nPowerMeterCheck = 0
                End If

            Case 35
                pbPowerMeter_Top(2) = False
                pnPowerMeterIndex(2) = 1
                nPowerMeterCheck = 0

            Case 40
                If pbPowerMeterOn(0) = True And pbPowerMeter_Top(0) = False Then 'Check Power Meter 1 Use
                    pbPowerMeterLaserOff(0) = True
                End If
                If pbPowerMeterOn(1) = True And pbPowerMeter_Top(1) = False Then 'Check Power Meter 2 Use
                    pbPowerMeterLaserOff(1) = True
                End If
                If pbPowerMeterOn(2) = True And pbPowerMeter_Top(2) = False Then 'Check Power Meter 3 Use
                    pbPowerMeterLaserOff(2) = True
                End If
                If pbPowerMeterOn(3) = True And pbPowerMeter_Top(3) = True Then 'Check Power Meter 4 Use
                    pbPowerMeterLaserOff(3) = True
                End If
                nPowerMeterCheck = 41

            Case 41
                If pbPowerMeterOn(0) = True And pbPowerMeter_Top(0) = True Then 'Check Power Meter 1 Off
                    nPowerMeterCheck = 42
                ElseIf pbPowerMeterOn(0) = False Then
                    nPowerMeterCheck = 42
                End If

            Case 42
                If pbPowerMeterOn(1) = True And pbPowerMeter_Top(1) = True Then 'Check Power Meter 2 Off
                    nPowerMeterCheck = 43
                ElseIf pbPowerMeterOn(1) = False Then
                    nPowerMeterCheck = 23
                End If

            Case 43
                If pbPowerMeterOn(2) = True And pbPowerMeter_Top(2) = True Then 'Check Power Meter 3 Off
                    nPowerMeterCheck = 44
                ElseIf pbPowerMeterOn(2) = False Then
                    nPowerMeterCheck = 44
                End If

            Case 44
                If pbPowerMeterOn(3) = False Then 'Check Power Meter 4 Use
                    nPowerMeterCheck = 45
                ElseIf pbPowerMeterOn(3) = True And pbPowerMeter_Top(3) = False Then
                    nPowerMeterCheck = 0
                End If

            Case 45
                pbPowerMeter_Top(3) = False
                pnPowerMeterIndex(3) = 1
                nPowerMeterCheck = 0

        End Select

        Exit Sub
SysErr:

    End Sub

End Class
