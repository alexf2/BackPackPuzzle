package pirates;

public interface JackSparrowHelper {
    /**
     * @param path            ѕолный путь к csv файлу с ценами и количествами (sources.csv), доступными в разных местах
     * @param numberOfGallons совокупное количество галлонов, нужное ƒжеку
     * @return <tt>Purchases</tt> детальный совет ƒжеку, где и в каком количестве покупать ром
     */
    Purchases helpJackSparrow(String path, int numberOfGallons);
}
