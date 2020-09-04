namespace Capture.Core
{
    /// <summary>
    /// ��������� �������.
    /// </summary>
    public interface ICaptureSettings
    {
        /// <summary>
        /// ���� �� ����� ��������.
        /// </summary>
        string OutputPath { get; set; }

        /// <summary>
        /// ��� ������� �����.
        /// </summary>
        AreaKind AreaKind { get; set; }

        /// <summary>
        /// ������������ ������� �������.
        /// </summary>
        string AreaName { get; set; }

        /// <summary>
        /// ���������� ��� ���. ��� ��������� ������.
        /// </summary>
        BitRate Rate { get; set; }

        /// <summary>
        /// ���������� ������ � �������.
        /// </summary>
        int Fps { get; set; }

        ICaptureSettings Default { get; }
    }
}