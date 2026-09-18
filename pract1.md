# 1 задание
``` bash
my_pc@DESKTOP-APG2SS0:/etc$ grep -o '^[^:]*' passwd | sort
```

# 2 задание
``` bash
my_pc@DESKTOP-APG2SS0:/etc$ grep -v '^#' protocols | awk '{print $2, $1}' | sort -nr | head -n 5
```

# 3 задание
``` bash
my_pc@DESKTOP-APG2SS0:~$ touch banner
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