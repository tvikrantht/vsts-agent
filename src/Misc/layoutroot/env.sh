#!/bin/bash

varCheckList=('LANG' 'JAVA_HOME' 'ANT_HOME' 'M2_HOME' 'MYSQL_PATH')
envContents=""

if [ -f ".Env" ]; then
    envContents=`cat .Env`
else
    touch .Env
fi

# echo Contents:
# echo ${envContents}

function writeVar()
{
    checkVar="$1"
    checkDelim="${1}="
    if test "${envContents#*$checkDelim}" != "$envContents"
    then
        echo "Contents contains ${checkVar}"
        
    else
        echo "Does not contain ${checkVar}"
        if [ ! -z "${!checkVar}" ]; then
            echo "${checkVar}=${!checkVar}">>.Env
        fi
    fi    
}

echo $PATH>.Path

for var_name in ${varCheckList[@]}
do
    writeVar "${var_name}"
done
