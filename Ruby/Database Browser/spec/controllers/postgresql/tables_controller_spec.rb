require 'rails_helper'

RSpec.describe PostgreSQL::TablesController, type: :controller do

  describe 'GET index' do
    context 'existent database' do
      before { get :index, account_slug: 'nice-co', deployment_id: 1234, database_name: 'booktown' }
      it { expect(response.status).to eq(200) }
    end

    context 'inexistent database' do
      before { get :index, account_slug: 'nice-co', deployment_id: 1234, database_name: 'hello' }
      it { expect(response.status).to eq(404) }
    end
  end

end
