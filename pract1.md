# 1 задание. Вывести отсортированный по алфавиту список из etc/passwd
``` bash
my_pc@DESKTOP-APG2SS0:/etc$ grep -o '^[^:]*' passwd | sort
```

# 2 задание. 5 наибольших портов/протоколов из /etc/protocols
``` bash
my_pc@DESKTOP-APG2SS0:/etc$ grep -v '^#' protocols | awk '{print $2, $1}' | sort -nr | head -n 5
```

# 3 задание. Баннер
``` bash
my_pc@DESKTOP-APG2SS0:~$ nano banner
```

## Код "banner"
``` bash
text=$1
charCount=${#text}

echo -n "+"
for ((i=0; i<$charCount+2; i++)); do echo -n "-"; done
echo "+"

echo -n "| $text"
echo " |"

echo -n "+"
for ((i=0; i<$charCount+2; i++)); do echo -n "-"; done
echo "+"
```

``` bash
my_pc@DESKTOP-APG2SS0:~$ chmod +x banner
my_pc@DESKTOP-APG2SS0:~$ ./banner "I love MIREA"
```

# 4 задание. Вывод ключевых слов из c/py/java файлов (без повторений)
``` bash
my_pc@DESKTOP-APG2SS0:~$ vim hello.c
```

## Код cpp файла
``` cpp
#include<stdio.h>
int main(){
	cout << "Hello, world!" << endl;
	return 0;
}
```

``` bash
my_pc@DESKTOP-APG2SS0:~$ grep -oE '[a-zA-Z_][a-zA-Z0-9_]*' hello.c | sort -u | tr '\n' ' ' && echo
```

# 5 задание. Регистрация пользовательской команды в /usr/local/bin (скрипт reg)
``` bash
my_pc@DESKTOP-APG2SS0:~$ nano reg
```

``` bash
chmod +x "$1"
cp "$1" /usr/local/bin/
echo "Успешно! Команда '$1' зарегистрирована и доступна как $(basename "$1")."
```

``` bash
my_pc@DESKTOP-APG2SS0:~$ chmod +x reg
my_pc@DESKTOP-APG2SS0:~$ echo -e '#!/bin/bash\necho "Hello from world!"' > hi_world
my_pc@DESKTOP-APG2SS0:~$ sudo ./reg banner
```

# 6 задание. Проверка наличия комментария в первой строке c/py/java файлов
``` bash
text=$1
read -r -n 1 FIRST_CHAR < "$text"

ext="${text##*.}"

if [[ $FIRST_CHAR == "#" && $ext == "py" ]]; then
    echo "Первая строка py файла содержит комментарий"
elif [[ $FIRST_CHAR == "/" && ( $ext == "c" || $ext == "js" ) ]]; then
    echo "Первая строка c или js файла содержит комментарий"
else
    echo "В первой строке не найден комментарий"
fi
```

# 7 задание. Нахождение файлов-дубликатов по содержимому
``` bash
dir="${1:-.}"

find "$dir" -type f -exec sha256sum {} + 2>/dev/null | \
    sort | \
    awk '{
        hash = $1
        $1 = ""
        sub(/^[ \t]+/, "")
        files[hash] = files[hash] ? files[hash] "\n  " $0 : $0
        count[hash]++
    }
    END {
        for (h in count) {
            if (count[h] > 1) {
                print "Hash: " h "\n  " files[h] "\n"
            }
        }
    }'
```

# Задача 8. Поиск файлов по расширению и архивация в tar
``` bash
set -euo pipefail

dir="${1:?Укажите каталог}"
ext="${2:?Укажите расширение без точки}"
archive_name="archive_${ext}.tar"

find "$dir" -maxdepth 1 -type f -name "*.${ext}" -print0 | tar -cvf "$archive_name" --null -T -
echo "Архив '$archive_name' создан"
```

# Задача 9. Замена 4 пробелов на символ TAB
``` bash
set -euo pipefail

if [ "$#" -ne 2 ]; then
    echo "Использование: $0 <входной_файл> <выходной_файл>" >&2
    exit 1
fi

sed 's/    /\t/g' "$1" > "$2"
```

``` bash
./spaces_to_tabs.sh source_spaces.py converted_tabs.py
cat -A converted_tabs.py
```

# Задача 10. Названия пустых текстовых файлов в указанной директории
``` bash
dir="${1:?Укажите директорию}"

find "$dir" -maxdepth 1 -type f -empty -exec sh -c '
    for f do
        mime=$(file -b --mime-type "$f")
        if [ "$mime" = "text/plain" ] || [ "$mime" = "inode/x-empty" ]; then
            basename "$f"
        fi
    done
' sh {} +
```
