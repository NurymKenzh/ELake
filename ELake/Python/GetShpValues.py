import shapefile

file=r'E:\Documents\Google Drive\New\Layers\my\Lakes.shp'
field = 'name'
##file = raw_input()
##field = raw_input()

sf = shapefile.Reader(file)
fieldIndex = 1
currentIndex = 0
for sr in sf.fields:
##    print "Field: ", sr[0]
    currentIndex += 1
    if field == sr[0]:
        fieldIndex = currentIndex
        break

fieldIndex = fieldIndex - 1
##print fieldIndex

values = []
for sr in sf.shapeRecords():
##    print type(sr.record)
    values.append(sr.record[fieldIndex-1:fieldIndex][0])
##    print '[%s]' % ', '.join(map(str, sr.record))

print '[%s]' % ', '.join(map(str, values))
