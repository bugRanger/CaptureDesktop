namespace Capture.Core
{
    using System;

    [Flags]
    public enum CaptureInfObjects : int
    {
        // TODO Fix encoding chars.
        /// <summary>
        /// ��� ����������.
        /// </summary>
        None = 0,
        /// <summary>
        /// ���������� � �������.
        /// </summary>
        Cursor = 1 << 1,
        /// <summary>
        /// ���������� � ��������.
        /// </summary>
        Keys = 1 << 2,
        /// <summary>
        /// ������� ����������.
        /// </summary>
        Default = Cursor | Keys,
        /// <summary>
        /// ������ ����������.
        /// </summary>
        Fully = -1,
    }
}