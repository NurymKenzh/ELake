import shapefile

file = raw_input()
sf = shapefile.Reader(file)
field = raw_input()
fieldIndex = 1
currentIndex = 0

for sr in sf.fields:
##    print "Field: ", sr[0]
    currentIndex += 1
    if field == sr[0]:
        fieldIndex = currentIndex
        break

##print fieldIndex

values = []

for sr in sf.shapeRecords():
##    values.append(sr.record[1:fieldIndex][0])
    values.append(sr.record[fieldIndex:fieldIndex + 1][0])

print values
