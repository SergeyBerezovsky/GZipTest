Реализация тестового задания от Veeam Software

Сжатие и распаковка происходит в многопоточной среде, где количество потоков соответствует количеству ядер.
При сжатии считывается полная длина файла, в процессе файл разбивается на блоки одинакового размера 1 мб.
Размеры сжатых частей сохраняются в отдельном индексном файле.В этом случае скорость распаковки многократно увеличивается.
