# TextHandler (Тестовое задание 1. Обработчик текста)
Программа принимает на вход файлы и удаляет в них слова длиной менее N числа символов и указанные пользователем символы. Файлы можно сохранить в желаемую директорию. Доступные расширения файлов: .txt, .pdf, .docx, .doc.

Достоинства:
1. Для реализации архитектуры использован MVVM паттерн;
2. Программа отображает прогресс обработки файлов в UI элементе "ProgressBar";
3. Для обработки текста использована библиотека OpenNLP (на базе машинного обучнеия).
4. Использованны асинхронные методы, благодаря чему интерфейс не виснет при обработке файлов.

Недостатки:
1. Не все файлы с раширением .doc успешно открываются.
2. Нет Unit-тестов
