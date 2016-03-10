require 'priority_queue' # requires gem install PriorityQueue

class Dijkstra

  def initialize(vertices)
    @edges = vertices.inject({}) do |r, (v1, v2, d)|
      (r[v1] ||= []) << [v2, d]
      (r[v2] ||= []) << [v1, d]
      r
    end
  end

  def calc_shortest(source, dest)
    dist, prev = Hash.new(Float::INFINITY), {}
    visited = []

    nodes = PriorityQueue.new
    nodes[source] = 0
    dist[source] = 0

    until nodes.empty?
      u, d = nodes.delete_min

      @edges[u].each do |v, dv2|
        next if visited.include? v
        alt = d + dv2

        if alt < dist[v]
          nodes[v] = dist[v] = alt
          prev[v] = u
        end
      end

      visited << u
      break if u == dest
    end

    {shortest: d, distance: dist, traverse: prev}
  end

end