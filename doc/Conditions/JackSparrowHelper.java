package pirates;

public interface JackSparrowHelper {
    /**
     * @param path            ������ ���� � csv ����� � ������ � ������������ (sources.csv), ���������� � ������ ������
     * @param numberOfGallons ���������� ���������� ��������, ������ �����
     * @return <tt>Purchases</tt> ��������� ����� �����, ��� � � ����� ���������� �������� ���
     */
    Purchases helpJackSparrow(String path, int numberOfGallons);
}
