import glob
import shapefile

folder = raw_input()
outfile = raw_input()
files = glob.glob(folder + "/*.shp")
w = shapefile.Writer()
for f in files:
	r = shapefile.Reader(f)
	w._shapes.extend(r.shapes())
	w.records.extend(r.records())

w.fields = list(r.fields)
w.save(outfile)
