require 'rails_helper'

RSpec.describe PostgreSQL::Deployment, type: :model do

  let (:dbs) do
    PostgreSQL::Deployment.new(
        type: 'postgresql',
        host: 'localhost',
        port: 5432,
        name: 'funky-deployment-9',
        account_id: 1337
    ).databases
  end

  it 'returns a list of databases of a deployment including booktown db' do
    expect(dbs.map(&:name)).to include('booktown')
  end


end
